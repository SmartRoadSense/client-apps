using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SmartRoadSense.Resources;
using SmartRoadSense.Shared.Api;
using SmartRoadSense.Shared.Data;
using SmartRoadSense.Shared.Database;

namespace SmartRoadSense.Shared {

    /// <summary>
    /// Manages data synchronization between client and remote server.
    /// </summary>
    public partial class SyncManager {

        /// <summary>
        /// Maximum number of points per upload chunk.
        /// </summary>
        const int ChunkSize = 1000;

        #region Timing constants

        /// <summary>
        /// Maximum amount of time between synchronization attempts.
        /// </summary>
        #if DEBUG
        public static readonly TimeSpan MaxSynchronizationInterval = TimeSpan.FromMinutes(2);
        #else
        public static readonly TimeSpan MaxSynchronizationInterval = TimeSpan.FromDays(1);
        #endif

        /// <summary>
        /// Minimum amount of time between synchronization attempts.
        /// </summary>
        #if DEBUG
        public static readonly TimeSpan MinSynchronizationInterval = TimeSpan.FromMinutes(1);
        #else
        public static readonly TimeSpan MinSynchronizationInterval = TimeSpan.FromMinutes(10);
        #endif

        /// <summary>
        /// Gets the next synchronization attempt window opening.
        /// </summary>
        public static DateTime NextUploadOpportunity {
            get {
                return Settings.LastUploadAttempt.Add(MinSynchronizationInterval);
            }
        }

        /// <summary>
        /// Gets the next synchronization attempt deadline.
        /// </summary>
        public static DateTime NextUploadDeadline {
            get {
                return Settings.LastUploadAttempt.Add(MaxSynchronizationInterval);
            }
        }

        #endregion

        #region Status

        public bool IsSyncing { get; private set; } = false;

        public event EventHandler StatusChanged;

        public event EventHandler<SyncErrorEventArgs> SyncError;

        #endregion

        /// <summary>
        /// Gets whether the manager can and should sync.
        /// May return false because sync process is already running or because the last
        /// synchronization attempt was too soon.
        /// </summary>
        public bool CheckSyncConditions(SyncPolicy policy) {
#if DEBUG
            if(policy != SyncPolicy.Forced) {
                Log.Debug("Unforced sync aborted in debug configuration");
                return false;
            }
#endif

            if(Settings.OfflineMode) {
                Log.Debug("Can't sync: synchronization disabled in offline mode");
                return false;
            }

            if (IsSyncing) {
                Log.Debug("Can't sync: SyncManager already syncing");
                return false;
            }

            return CheckPlatformSyncConditions();
        }

        /// <summary>
        /// Runs the synchronization attempt.
        /// Does not raise exceptions.
        /// </summary>
        public async Task<SyncResult> Synchronize(CancellationToken token, SyncPolicy policy = SyncPolicy.Default) {
            if (DateTime.UtcNow < NextUploadOpportunity && policy == SyncPolicy.Default) {
                Log.Debug("Can't sync: sync attempt too early, next upload scheduled after {0}", NextUploadOpportunity);

                return new SyncResult(new InvalidOperationException("Sync attempt too early"));
            }

            Settings.LastUploadAttempt = DateTime.UtcNow;

            if (!CheckSyncConditions(policy)) {
                return new SyncResult(error: new InvalidOperationException("Sync conditions not met"));
            }

            Log.Debug("Sync attempt started (policy {0})", policy);

            IsSyncing = true;
            StatusChanged.Raise(this);

            try {
                var ret = await SynchronizeInner(token, policy);

                Log.Debug("Sync process terminated normally");
                Log.Event("Sync.terminate", new Dictionary<string, string>() {
                    { "policy", policy.ToString() }
                });

                if (ret.HasFailed) {
                    UserLog.Add(UserLog.Icon.Error, LogStrings.FileUploadFailure, ret.Error.Message);

                    SyncError.Raise(this, new SyncErrorEventArgs(ret.Error));
                }
                else {
                    if(ret.ChunksUploaded == 1) {
                        UserLog.Add(LogStrings.FileUploadSummarySingular);
                    }
                    else {
                        UserLog.Add(LogStrings.FileUploadSummaryPlural, ret.ChunksUploaded);
                    }
                }

                return ret;
            }
            catch(TaskCanceledException) {
                Log.Debug("Sync process was canceled");
                return new SyncResult(0, 0);
            }
            catch(Exception ex) {
                Log.Error(ex, "Sync process failed with unforeseen error");

                UserLog.Add(UserLog.Icon.Error, LogStrings.FileUploadFailure, ex.Message);

                return new SyncResult(error: ex);
            }
            finally {
                IsSyncing = false;
                StatusChanged.Raise(this);
            }
        }

        /// <summary>
        /// Inner worker synchronizer.
        /// Will never throw for normal sync operations, even in case of failure.
        /// </summary>
        /// <exception cref="TaskCanceledException">Thrown when sync canceled.</exception>
        private async Task<SyncResult> SynchronizeInner(CancellationToken token, SyncPolicy policy) {
            token.ThrowIfCancellationRequested();

            IList<DatabaseQueries.TrackAndCount> tracks = await Task.Run(() => {
                using(var db = DatabaseUtility.OpenConnection()) {
                    return (from t in db.GetAllPendingTracks()
                            let trackFilepath = FileNaming.GetDataTrackFilepath(t.TrackId)
                            where File.Exists(trackFilepath)
                            select t).ToList();
                }
            });
            if (tracks.Count == 0) {
                Log.Debug("No files to synchronize");
                return new SyncResult(0, 0);
            }

            token.ThrowIfCancellationRequested();

            Log.Debug("{0} tracks queued to synchronize", tracks.Count);
            Log.Event("Sync.start", new Dictionary<string, string>() {
                { "policy", policy.ToString() }
            });

            if(policy == SyncPolicy.ForceLast && tracks.Count > 1) {
                Log.Debug("Constraining upload to most recent file as per policy");
                tracks = new DatabaseQueries.TrackAndCount[] { tracks.First() };
            }

            int countUploadedPoints = 0;
            int countUploadedChunks = 0;

            foreach(var track in tracks) {
                token.ThrowIfCancellationRequested();

                int pendingPoints = track.DataCount - track.UploadedCount;
                Log.Debug("Uploading {0}/{1} points from track {2}", pendingPoints, track.DataCount, track.TrackId);

                try {
                    var reader = new DataReader(track.TrackId);
                    if(!await reader.Skip(track.UploadedCount)) {
                        Log.Error(null, "Cannot advance {0} rows in file for track {1}", track.UploadedCount, track.TrackId);
                        continue;
                    }

                    int currentChunk = 0;
                    while(pendingPoints > 0) {
                        int chunkPoints = Math.Min(ChunkSize, pendingPoints);
                        Log.Debug("Processing chunk {0} with {1} points", currentChunk + 1, chunkPoints);

                        var package = new List<DataPiece>(chunkPoints);
                        for(int p = 0; p < chunkPoints; ++p) {
                            if(!await reader.Advance()) {
                                throw new Exception(string.Format("Cannot read line for {0}th point", p + 1));
                            }
                            package.Add(new DataPiece {
                                TrackId = track.TrackId,
                                StartTimestamp = new DateTime(reader.Current.StartTicks),
                                EndTimestamp = new DateTime(reader.Current.EndTicks),
                                Ppe = reader.Current.Ppe,
                                PpeX = reader.Current.PpeX,
                                PpeY = reader.Current.PpeY,
                                PpeZ = reader.Current.PpeZ,
                                Latitude = reader.Current.Latitude,
                                Longitude = reader.Current.Longitude,
                                Bearing = reader.Current.Bearing,
                                Accuracy = reader.Current.Accuracy,
                                Vehicle = track.VehicleType,
                                Anchorage = track.AnchorageType,
                                NumberOfPeople = track.NumberOfPeople
                            });
                        }

                        var secret = Crypto.GenerateSecret();
                        var secretHash = secret.ToSha512Hash();

                        var uploadQuery = new UploadDataQuery {
                            Package = package,
                            SecretHash = secretHash
                        };
                        var response = await uploadQuery.Execute(token);
                        Log.Debug("Points uploaded successfully, chunk {0} for track ID {1}", currentChunk + 1, track.TrackId);

                        //Store record of uploaded chunk
                        using(var db = DatabaseUtility.OpenConnection()) {
                            var record = new TrackUploadRecord {
                                TrackId = track.TrackId,
                                UploadedId = response.UploadedTrackId,
                                Secret = secret,
                                UploadedOn = DateTime.UtcNow,
                                Count = chunkPoints
                            };

                            db.Insert(record);
                        }

                        pendingPoints -= chunkPoints;
                        currentChunk++;

                        countUploadedPoints += chunkPoints;
                        countUploadedChunks++;
                    }
                }
                catch(IOException exIo) {
                    Log.Error(exIo, "File for track {0} not found", track.TrackId);
                }
                catch(Exception ex) {
                    Log.Error(ex, "Failed while processing track {0}", track.TrackId);
                }
            }

            return new SyncResult(countUploadedPoints, countUploadedChunks);
        }

    }

}
