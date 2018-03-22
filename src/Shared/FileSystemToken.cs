using System;
using System.IO;
using System.Threading.Tasks;

#if WINDOWS_PHONE_APP
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
#else
//Disable useless async keyword warning for multi-platform code
#pragma warning disable 1998
#endif

namespace SmartRoadSense.Shared {

    /// <summary>
    /// Represents a platform-independent file token.
    /// </summary>
    public class FileSystemToken {

#if __ANDROID__ || __IOS__ || DESKTOP
        public FileSystemToken(string path) {
            _path = path;
        }

        private readonly string _path;

        private FileInfo _info = null;

        protected FileInfo Info {
            get {
                if (_info == null)
                    _info = new FileInfo(_path);

                return _info;
            }
        }
#elif WINDOWS_PHONE_APP
        public FileSystemToken(StorageFile file, BasicProperties properties) {
            _file = file;
            _basicProperties = properties;
        }

        private readonly StorageFile _file;
        private readonly BasicProperties _basicProperties;
#else
#error Unrecognized platform
#endif

        public override string ToString() {
#if __ANDROID__ || __IOS__ || DESKTOP
            return _path;
#elif WINDOWS_PHONE_APP
            return _file.Path;
#else
#error Unrecognized platform
#endif
        }

        /// <summary>
        /// Deletes the folder item.
        /// </summary>
        public async Task Delete() {
#if __ANDROID__ || __IOS__ || DESKTOP
            File.Delete(_path);
#elif WINDOWS_PHONE_APP
            await _file.DeleteAsync(StorageDeleteOption.PermanentDelete);
#endif
        }

        /// <summary>
        /// Opens the folder item for shared reading.
        /// </summary>
        public async Task<Stream> OpenReadStream() {
#if __ANDROID__ || __IOS__ || DESKTOP
            return new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
#elif WINDOWS_PHONE_APP
            return await _file.OpenStreamForReadAsync();
#endif
        }

        /// <summary>
        /// Gets a name representing the folder item.
        /// </summary>
        public string Filename {
            get {
#if __ANDROID__ || __IOS__ || DESKTOP
                return Info.Name;
#elif WINDOWS_PHONE_APP
                return _file.Name;
#endif
            }
        }

        /// <summary>
        /// Gets the item's creation time in the local timezone.
        /// </summary>
        public DateTime LocalCreationTime {
            get {
#if __ANDROID__ || __IOS__ || DESKTOP
                return Info.CreationTime;
#elif WINDOWS_PHONE_APP
                return _file.DateCreated.LocalDateTime;
#endif
            }
        }

        /// <summary>
        /// Gets the item's size in bytes.
        /// </summary>
        public int Size {
            get {
#if __ANDROID__ || __IOS__ || DESKTOP
                return (int)(Info.Length);
#elif WINDOWS_PHONE_APP
                if (_basicProperties == null)
                    return 0;

                return (int)_basicProperties.Size;
#endif
            }
        }

    }

}
