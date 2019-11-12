using System;
using System.IO;
using System.Threading.Tasks;

//Disable useless async keyword warning for multi-platform code
#pragma warning disable 1998
namespace SmartRoadSense {

    /// <summary>
    /// Represents a platform-independent file token.
    /// </summary>
    public class FileSystemToken {

        private readonly string _path;

        public FileSystemToken(string path) {
            _path = path;
        }

        private FileInfo _info;
        protected FileInfo Info {
            get {
                if (_info == null)
                    _info = new FileInfo(_path);

                return _info;
            }
        }

        public override string ToString() {
            return _path;
        }

        /// <summary>
        /// Deletes the folder item.
        /// </summary>
        public async Task Delete() {
            File.Delete(_path);
        }

        /// <summary>
        /// Opens the folder item for shared reading.
        /// </summary>
        public async Task<Stream> OpenReadStream() {
            return new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        /// Gets a name representing the folder item.
        /// </summary>
        public string Filename => Info.Name;

        /// <summary>
        /// Gets the item's creation time in the local timezone.
        /// </summary>
        public DateTime LocalCreationTime =>  Info.CreationTime;

        /// <summary>
        /// Gets the item's size in bytes.
        /// </summary>
        public int Size => (int)Info.Length;

    }
}
