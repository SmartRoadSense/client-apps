using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartRoadSense {

    /// <summary>
    /// Data management, parsing and conversion functions.
    /// </summary>
    public static class DataStore {

        /// <summary>
        /// Gets the current entries in the data store.
        /// </summary>
        public static Task<IList<FileSystemToken>> GetEntries() {
            return FileOperations.EnumerateFolderAsync(FileNaming.DataQueuePath, FileNaming.DataFileExtension);
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
