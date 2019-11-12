using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartRoadSense {

    /// <summary>
    /// Manages data synchronization between client and remote server.
    /// </summary>
    public class SyncManager {

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

                return new SyncResult(error: new InvalidOperationException("Sync attempt too early"));
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
                    { "policy", policy.ToString() },
                    { "uploadedCount", ret.DataPiecesUploaded.ToString(CultureInfo.InvariantCulture) },
                    { "deletedCount", ret.DataPiecesDeleted.ToString(CultureInfo.InvariantCulture) }
                });

                if (ret.HasFailed) {
                    UserLog.Add(UserLog.Icon.Error, AppResources.FileUploadFailure, ret.Error.Message);

                    SyncError.Raise(this, new SyncErrorEventArgs(ret.Error));
                }
                else {
                    if(ret.DataPiecesUploaded == 1) {
                        UserLog.Add(AppResources.FileUploadSummarySingular);
                    }
                    else {
                        UserLog.Add(AppResources.FileUploadSummaryPlural, ret.DataPiecesUploaded);
                    }
                }

                return ret;
            }
            catch(TaskCanceledException) {
                Log.Debug("Sync process was canceled");
                return new SyncResult();
            }
            catch(Exception ex) {
                Log.Error(ex, "Sync process failed with unforeseen error");

                UserLog.Add(UserLog.Icon.Error, AppResources.FileUploadFailure, ex.Message);

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

            var files = await FileOperations.EnumerateFolderAsync(FileNaming.DataQueuePath, FileNaming.DataFileExtension);
            if (files.Count == 0) {
                Log.Debug("No files to synchronize");
                return new SyncResult();
            }

            Log.Debug("{0} files queued to synchronize", files.Count);
            Log.Event("Sync.start", new Dictionary<string, string>() {
                { "policy", policy.ToString() },
                { "fileCount", files.Count.ToString(CultureInfo.InvariantCulture) }
            });

            if(policy == SyncPolicy.ForceLast && files.Count > 1) {
                Log.Debug("Constraining upload to most recent file as per policy");
                files = new FileSystemToken[] { files.First() };
            }

            int uploadedFiles = 0;
            int deletedFiles = 0;

            foreach(var file in files) {
                token.ThrowIfCancellationRequested();

                try {
                    var package = await file.ParsePackage();
                    if(package == null) {
                        await file.Delete();
                        ++deletedFiles;

                        continue;
                    }

                    if(!await TrackIsConsistent(package.Pieces)) {
                        Log.Debug("Ignoring inconsistent file");
                        continue;
                    }

                    var secret = Crypto.GenerateSecret();
                    var secretHash = secret.ToSha512Hash();

                    var uploadQuery = new UploadDataQuery {
                        Package = package,
                        SecretHash = secretHash
                    };
                    var response = await uploadQuery.Execute(token);

                    Log.Debug("File {0} uploaded successfully, uploaded track ID {1}", file, response.UploadedTrackId);
                    ++uploadedFiles;

                    //Done with this file
                    await file.Delete();
                    ++deletedFiles;

                    //Store record of uploaded track
                    using var db = DatabaseUtility.OpenConnection();
                    var record = new TrackUploadRecord
                    {
                        TrackId = package.Pieces.First().TrackId,
                        UploadedId = response.UploadedTrackId,
                        Secret = secret,
                        UploadedOn = DateTime.UtcNow
                    };

                    db.Insert(record);
                }
                catch(WebException ex) {
                    //Server down or network not working
                    //Stop everything and move on
                    return new SyncResult(dataPiecesUploaded: uploadedFiles, dataPiecesDeleted: deletedFiles, error: ex);
                }
                catch(ProtocolViolationException ex) {
                    //This probably is only a minor issue with this file
                    Log.Error(ex, "Protocol error while uploading file {0}", file);
                }
                catch(IOException ex) {
                    //This probably is a file access issue, just ignore it and go on
                    Log.Error(ex, "IO access error on file {0}", file);
                }
                catch(TaskCanceledException) {
                    //Stop here, handle at upper method
                    throw;
                }
                catch(Exception ex) {
                    //Catch-all exception handler other stuff
                    //Just stop everything now and log - we'll get back to this file
                    Log.Error(ex, "Failed while processing file {0}", file);

                    return new SyncResult(dataPiecesUploaded: uploadedFiles, dataPiecesDeleted: deletedFiles, error: ex);
                }
            }

            return new SyncResult(dataPiecesUploaded: uploadedFiles, dataPiecesDeleted: deletedFiles);
        }

        /// <summary>
        /// Verifies whether the track represented by a sequence of data pieces is consistent
        /// (i.e. it is composed of pieces from the same recording and has never been uploaded).
        /// </summary>
        private Task<bool> TrackIsConsistent(IList<DataPiece> pieces) {
            return Task.Run<bool>(() => {
                if(pieces == null || pieces.Count < 1)
                    return false;

                var firstId = pieces.First().TrackId;
                if(pieces.Any(p => p.TrackId != firstId)) {
                    Log.Error(null, "Inconsistent recording: pieces with multiple Track IDs in same file");
                    return false;
                }

                return true;
            });
        }

		private bool CheckPlatformSyncConditions()
		{
			if (Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.None)
			{
				Log.Debug("Can't sync: no available connection");
				return false;
			}

			return true;
		}
	}

}
