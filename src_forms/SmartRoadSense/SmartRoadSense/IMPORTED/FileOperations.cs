using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable 1998

namespace SmartRoadSense {

    /// <summary>
    /// Common platform file operations.
    /// </summary>
    public static class FileOperations {

        /// <summary>
        /// Opens a writable stream to a newly created file.
        /// </summary>
        /// <remarks>
        /// Will throw if the file already exists.
        /// </remarks>
        public static async Task<Stream> CreateNewFile(string path) {
            return new FileStream(path, FileMode.CreateNew);
        }

        /// <summary>
        /// Opens a writable stream to an empty file.
        /// </summary>
        public static async Task<Stream> CreateOrTruncateFile(string path) {
            return new FileStream(path, FileMode.Create);
        }

        /// <summary>
        /// Opens a readable stream to an existing file.
        /// </summary>
        public static async Task<Stream> ReadFile(string path) {
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        /// Checks whether a file exists at a given path.
        /// </summary>
        public static async Task<bool> CheckFile(string path) {
            return File.Exists(path);
        }

        public static async Task<FileSystemToken> GetToken(string path) {
            return new FileSystemToken(path);
        }

        /// <summary>
        /// Retrieves a filtered enumeration of files in a folder.
        /// </summary>
        /// <param name="path">´Path to the folder in which to search.</param>
        /// <param name="extensionFilter">Optional filter parameter on filenames (matches file extension exactly).</param>
        public static async Task<IList<FileSystemToken>> EnumerateFolderAsync(string path, string extensionFilter) {
            return await Task.Run(() => {
                IEnumerable<string> files;
                if(string.IsNullOrEmpty(extensionFilter))
                    files = Directory.EnumerateFiles(path);
                else
                    files = Directory.EnumerateFiles(path, "*." + extensionFilter, SearchOption.TopDirectoryOnly);

                return (from f in files
                        orderby f descending
                        select new FileSystemToken(f)).ToList();
            });
        }
    }
}
