using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartRoadSense.Shared.Database;
using SmartRoadSense.Shared.DataModel;

namespace SmartRoadSense.Shared.Data {

    /// <summary>
    /// Data management, parsing and conversion functions.
    /// </summary>
    public static class DataStore {

        public static Task<IList<TrackInformation>> GetCollectedTracks() {
            return Task.Run<IList<TrackInformation>>(() => {
                using(var db = DatabaseUtility.OpenConnection()) {
                    var m = db.GetMapping<StatisticRecord>();
                    var records = db.Query<StatisticRecord>($"SELECT * FROM {m.TableName} ORDER BY {nameof(StatisticRecord.Start)} ASC");

                    return (from r in records
                            select new TrackInformation(
                                r.TrackId,
                                r.Start,
                                r.ElapsedTime,
                                r.DistanceTraveled
                            )).ToList();
                }
            });
        }

        public static Task<double[]> GetTrackPpe(Guid trackId) {
            return Task.Run(() => {
                var filePath = FileNaming.GetDataTrackFilepath(trackId);
                using(var s = File.Open(filePath, FileMode.Open, FileAccess.Read)) {
                    using(var reader = new StreamReader(s)) {

                    }
                }

                return new double[0];
            });
        }

        /// <summary>
        /// Gets the current entries in the data store.
        /// </summary>
        public static Task<IList<FileSystemToken>> GetEntries() {
            return FileOperations.EnumerateFolderAsync(FileNaming.DataQueuePath, FileNaming.DataQueueFileExtension);
        }

        /// <summary>
        /// Performs file parsing on a background thread.
        /// Returns null if the file is corrupted or cannot be parsed correctly.
        /// </summary>
        public static async Task<DataPackage> ParsePackage(this FileSystemToken fileToken) {
            using (Stream fileStream = await fileToken.OpenReadStream()) {
                return await Task.Run(() => {
                    try {
                        var parser = DataPackageParser.DetectParser(fileStream);
                        fileStream.Position = 0;

                        return parser.Parse(fileStream);
                    }
                    catch(JsonSerializationException ex) {
                        Log.Error(ex, "File {0} is corrupted and cannot be parsed", fileToken);
                        return null;
                    }
                });
            }
        }

        /// <summary>
        /// Performs file parsing on a background thread.
        /// Returns null if the file is corrupted or cannot be parsed correctly.
        /// </summary>
        public static async Task<DataPackageInfo> ParsePackageInfo(this FileSystemToken fileToken) {
            using (Stream fileStream = await fileToken.OpenReadStream()) {
                return await Task.Run(() => {
                    try {
                        var parser = DataPackageParser.DetectParser(fileStream);
                        fileStream.Position = 0;

                        return parser.ParseInfo(fileStream);
                    }
                    catch (JsonSerializationException ex) {
                        Log.Error(ex, "File {0} is corrupted and cannot be parsed", fileToken);
                        return null;
                    }
                });
            }
        }

        /// <summary>
        /// Deletes all queued data files.
        /// </summary>
        public static async Task DeleteAll() {
            var files = await FileOperations.EnumerateFolderAsync(FileNaming.DataQueuePath, string.Empty);
            foreach (var file in files) {
                try {
                    await file.Delete();
                    Log.Debug("Deleted file {0}", file);
                }
                catch(Exception ex) {
                    Log.Error(ex, "Cannot delete file {0}", file);
                }
            }
        }

        /// <summary>
        /// Attempts to parse a file and extract its list of data pieces.
        /// </summary>
        /// <returns>List of data pieces or null if the file could not be parsed.</returns>
        public static async Task<IList<DataPiece>> ExtractDataPieces(this FileSystemToken token) {
            using (var fileStream = await token.OpenReadStream()) {
                return await Task.Run(() => {
                    try {
                        var parser = DataPackageParser.DetectParser(fileStream);
                        fileStream.Seek(0, SeekOrigin.Begin);

                        var package = parser.Parse(fileStream);
                        return package.Pieces;
                    }
                    catch (Exception ex) {
                        Log.Error(ex, "Failed to parse file {0}", token);
                        return null;
                    }
                });
            }
        }

    }

}
