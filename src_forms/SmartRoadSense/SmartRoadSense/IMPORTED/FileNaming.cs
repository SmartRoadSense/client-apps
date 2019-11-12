using System;
using System.IO;
using System.Threading.Tasks;

namespace SmartRoadSense {

    /// <summary>
    /// Collects information about file naming and path generation.
    /// </summary>
    public static class FileNaming {

        private static string[] GetInitializedFolderPaths() {
            DataQueuePath = Path.Combine(BaseDocumentsFolder, DataQueueFolder);

            return new string[] {
                DataQueuePath
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Task InitializeFileStructure()
        {
            return Task.Run(() =>
            {
                var paths = GetInitializedFolderPaths();

                foreach(var path in paths)
                {
                    Directory.CreateDirectory(path);
                }
            });
        }

        private static string _baseDocumentFolder;

        /// <summary>
        /// 
        /// </summary>
        private static string BaseDocumentsFolder {
            get {
                if (_baseDocumentFolder == null) {
                    _baseDocumentFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }

                return _baseDocumentFolder;
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

        public const string DataFileExtension = "srs";

        private const string DataFileDatePattern = "yyyy-MM-dd-HH-mm-ss";

        /// <summary>
        /// Generates a filename for a data file.
        /// </summary>
        public static string GenerateDataFileName() {
            //NOTE: uses LOCAL time instead of UTC because it makes more sense on a local
            //      filesystem. File contents use UTC times.
            return string.Concat(DateTime.Now.ToString(DataFileDatePattern), ".", DataFileExtension);
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
