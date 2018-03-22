using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_PHONE_APP
using Windows.Storage;
using Windows.Storage.Search;
#else
//Disable useless async keyword warning for multi-platform code
#pragma warning disable 1998
#endif

namespace SmartRoadSense.Shared {

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
#if WINDOWS_PHONE_APP
            var folderPath = Path.GetDirectoryName(path);
            var folder = await StorageFolder.GetFolderFromPathAsync(folderPath);

            var filename = Path.GetFileName(path);
            var file = await folder.CreateFileAsync(filename, CreationCollisionOption.FailIfExists);

            return await file.OpenStreamForWriteAsync();
#else
            return new FileStream(path, FileMode.CreateNew);
#endif
        }

        /// <summary>
        /// Opens a writable stream to an empty file.
        /// </summary>
        public static async Task<Stream> CreateOrTruncateFile(string path) {
#if WINDOWS_PHONE_APP
            var folderPath = Path.GetDirectoryName(path);
            var folder = await StorageFolder.GetFolderFromPathAsync(folderPath);

            var filename = Path.GetFileName(path);
            var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            return await file.OpenStreamForWriteAsync();
#else
            return new FileStream(path, FileMode.Create);
#endif
        }

        /// <summary>
        /// Opens a readable stream to an existing file.
        /// </summary>
        public static async Task<Stream> ReadFile(string path) {
#if WINDOWS_PHONE_APP
            var file = await StorageFile.GetFileFromPathAsync(path);

            return await file.OpenStreamForReadAsync();
#else
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
#endif
        }

        /// <summary>
        /// Checks whether a file exists at a given path.
        /// </summary>
        public static async Task<bool> CheckFile(string path) {
#if WINDOWS_PHONE_APP
            //NOTE: no other better way to do this?
            //      https://social.msdn.microsoft.com/Forums/en-US/1eb71a80-c59c-4146-aeb6-fefd69f4b4bb/how-to-detect-if-a-file-exists?forum=winappswithcsharp

            try {
                await StorageFile.GetFileFromPathAsync(path);

                return true;
            }
            catch(FileNotFoundException) {
                return false;
            }
#else
            return File.Exists(path);
#endif
        }

        public static async Task<FileSystemToken> GetToken(string path) {
#if WINDOWS_PHONE_APP
            var file = await StorageFile.GetFileFromPathAsync(path);
            var properties = await file.GetBasicPropertiesAsync();
            return new FileSystemToken(file, properties);
#else
            return new FileSystemToken(path);
#endif
        }

        /// <summary>
        /// Retrieves a filtered enumeration of files in a folder.
        /// </summary>
        /// <param name="path">´Path to the folder in which to search.</param>
        /// <param name="extensionFilter">Optional filter parameter on filenames (matches file extension exactly).</param>
        public static async Task<IList<FileSystemToken>> EnumerateFolderAsync(string path, string extensionFilter) {
#if __ANDROID__ || __IOS__ || DESKTOP
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
#elif WINDOWS_PHONE_APP
            var folder = await StorageFolder.GetFolderFromPathAsync(path);

            var files = from f in await folder.GetFilesAsync(CommonFileQuery.OrderByName)
                        where (string.IsNullOrEmpty(extensionFilter) || f.Name.EndsWith(extensionFilter))
                        orderby f.Name descending
                        select f;

            var ret = new List<FileSystemToken>();
            foreach(var f in files) {
                var properties = await f.GetBasicPropertiesAsync();
                ret.Add(new FileSystemToken(f, properties));
            }

            return ret;
#else
#error Unrecognized platform
#endif
        }

    }

}
