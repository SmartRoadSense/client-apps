using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

#if __IOS__
using Foundation;
#elif WINDOWS_PHONE_APP
using Windows.Storage;
#endif

namespace SmartRoadSense.Shared {

    /// <summary>
    /// Collects information about file naming and path generation.
    /// </summary>
    public static class FileNaming {

        private static string[] GetInitializedFolderPaths() {
            DataQueuePath = Path.Combine(BaseDocumentsFolder, DataQueueFolder);
            DataTracksPath = Path.Combine(BaseDocumentsFolder, TracksFolder);

            return new string[] {
                DataQueuePath,
                DataTracksPath
            };
        }

        /// <summary>
        /// Initializes the file and folder structure used by the application.
        /// </summary>
#if __ANDROID__
        public static Task InitializeFileStructure() {
            return Task.Run(() => {
                var paths = GetInitializedFolderPaths();

                foreach(var path in paths) {
                    var javaFile = new Java.IO.File(path);
                    if(javaFile.Exists()) {
                        if(javaFile.IsDirectory)
                            continue;
                        else
                            Log.Warning(new IOException(), "Cannot initialize directory {0} because non-directory file already exists on path", path);
                    } else {
                        javaFile.Mkdirs();
                    }
                }
            });
        }
#elif __IOS__ || DESKTOP
        public static Task InitializeFileStructure() {
            return Task.Run(() => {
                var paths = GetInitializedFolderPaths();

                foreach (var path in paths) {
                    Directory.CreateDirectory(path);
                }
            });
        }
#elif WINDOWS_PHONE_APP
        public static async Task InitializeFileStructure() {
            var paths = GetInitializedFolderPaths();

            foreach (var path in paths) {
                await CreateDirectoryIfNeeded(path);
            }
        }

        private static async Task CreateDirectoryIfNeeded(string path) {
            //This is the stupidest code ever, thank you WinRT
            var folderName = Path.GetFileNameWithoutExtension(path);

            var parentPath = Path.GetDirectoryName(path);
            var parentFolder = await StorageFolder.GetFolderFromPathAsync(parentPath);

            var folder = (from child in await parentFolder.GetItemsAsync()
                          where child.Name.Equals(folderName, StringComparison.OrdinalIgnoreCase)
                          select child).FirstOrDefault();

            if (folder == null) {
                await parentFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            }
        }
#else
#error Unrecognized platform
#endif

        private static string _baseDocumentFolder = null;

        private static string BaseDocumentsFolder {
            get {
                if (_baseDocumentFolder == null) {
#if __ANDROID__
                    _baseDocumentFolder = App.Context.FilesDir.AbsolutePath;
#elif __IOS__
                    var documents = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User) [0];
                    _baseDocumentFolder = documents.Path;
#elif WINDOWS_PHONE_APP
                    _baseDocumentFolder = ApplicationData.Current.LocalFolder.Path;
#elif DESKTOP
                    _baseDocumentFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
#else
#error Unrecognized platform
#endif
                }

                return _baseDocumentFolder;
            }
        }

        private static string _basePublicFolder = null;

        private static string BasePublicFolder {
            get {
                if(_basePublicFolder == null) {
#if __ANDROID__
                    _basePublicFolder = App.Context.GetExternalFilesDir(null).AbsolutePath; ;
#else
                    // TODO: CHECK IF FOLDER IS CORRECT
                    _basePublicFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
#endif
                }

                return _basePublicFolder;
            }
        }

        /// <summary>
        /// Name of the data queue folder.
        /// </summary>
        /// <remarks>
        /// Cannot change for backcompatibility reasons.
        /// </remarks>
        private const string DataQueueFolder = "sensing_folder";

        /// <summary>
        /// Gets the absolute path to the data queue folder.
        /// </summary>
        public static string DataQueuePath { get; private set; }

        public const string DataQueueFileExtension = "srs";

        private const string DataQueueFileDatePattern = "yyyy-MM-dd-HH-mm-ss";

        /// <summary>
        /// Generates a filename for a data file.
        /// </summary>
        public static string GenerateDataQueueFilename() {
            //NOTE: uses LOCAL time instead of UTC because it makes more sense on a local
            //      filesystem. File contents use UTC times.
            return string.Concat(DateTime.Now.ToString(DataQueueFileDatePattern), ".", DataQueueFileExtension);
        }

        private const string TracksFolder = "tracks";

        public static string DataTracksPath { get; private set; }

        public const string DataTracksFileExtension = "csv";

        /// <summary>
        /// Get file path for a session track file.
        /// </summary>
        public static string GetDataTrackFilepath(Guid session) {
            string filename = string.Concat(session.ToString("N"), ".", DataTracksFileExtension);
            return Path.Combine(FileNaming.DataTracksPath, filename);
        }

        private const string LogStoreFilename = "log.json";

        /// <summary>
        /// Gets the path to the log store.
        /// </summary>
        public static string LogStorePath {
            get {
                return Path.Combine(BaseDocumentsFolder, LogStoreFilename);
            }
        }

        private const string ErrorDumpFilename = "internal-error.dmp";

        /// <summary>
        /// Gets the path to the error dump file.
        /// </summary>
        public static string ErrorDumpPath {
            get {
                return Path.Combine(BaseDocumentsFolder, ErrorDumpFilename);
            }
        }

        private const string DatabaseFilename = "smartroadsense.db";

        /// <summary>
        /// Gets the path to the database file.
        /// </summary>
        public static string DatabasePath {
            get {
                return Path.Combine(BaseDocumentsFolder, DatabaseFilename);
            }
        }

    }

}
