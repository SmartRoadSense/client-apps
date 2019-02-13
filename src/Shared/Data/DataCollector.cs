using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartRoadSense.Resources;
using SmartRoadSense.Shared.Exceptions;

namespace SmartRoadSense.Shared.Data {

    /// <summary>
    /// Collects data and flushes it to disk in batches.
    /// </summary>
    public class DataCollector {

        /// <summary>
        /// Amount of data pieces that are serialized together.
        /// </summary>
        public const int SerializationBatchSize = 1000;

        /// <summary>
        /// Maximum number of data pieces collected by the collector before dropping information.
        /// </summary>
        public const int DropSize = 10000;

        private Queue<DataPiece> _data = new Queue<DataPiece>(SerializationBatchSize);

        /// <summary>
        /// Gets current size of data collected.
        /// </summary>
        public int Size {
            get {
                return _data.Count;
            }
        }

        /// <summary>
        /// Collects a new piece of data.
        /// </summary>
        public void Collect(DataPiece piece) {
            _data.Enqueue(piece);

            if (_data.Count >= SerializationBatchSize) {
                Flush();
            }

            //Remove elements until compliant with maximum size
            if(_data.Count >= DropSize) {
                Log.Warning(new DataLossException(), "Dropping data piece because collector is full");

                while(_data.Count >= DropSize) {
                    _data.Dequeue();
                }
            }
        }

        /// <summary>
        /// Flushes collected data to disk.
        /// </summary>
        public void Flush() {
            if (_data.Count == 0) {
                Log.Debug("Ignoring collector flush because no data collected");
                return;
            }

            //Kick off flushing on background thread
            FlushCoreAsync(_data.ToArray()).Forget();

            //NOTE: lock free switch, this MAY drop data in same cases,
            //      but this is preferable to locking on the queue
            _data = new Queue<DataPiece>(SerializationBatchSize);
        }

        /// <summary>
        /// Executes flush operation on a collection of data pieces asynchronously.
        /// </summary>
        private async Task FlushCoreAsync(ICollection<DataPiece> data) {
            var filename = FileNaming.GenerateDataFileName();
            string filepath = Path.Combine(FileNaming.DataQueuePath, filename);
            Log.Debug("Flushing collector with {0} data pieces to {1}", data.Count, filename);

            try {
                using(var fileStream = await FileOperations.CreateNewFile(filepath)) {
                    using (var textWrite = new StreamWriter(fileStream, Encoding.UTF8)) {
                        var serializer = JsonSerializer.Create();

                        await Task.Run(() => {
                            serializer.Serialize(textWrite, data, typeof(ICollection<DataPiece>));

                            //Early flush on background thread
                            textWrite.Flush();
                            fileStream.Flush();
                        });
                    }
                }
            }
            catch (Exception ex) {
                Log.Error(ex, "Failed flushing data collector to file {0}", filepath);
                UserLog.Add(UserLog.Icon.Error, LogStrings.FileWriteError);
            }

            UserLog.Add(UserLog.Icon.None, LogStrings.FileWriteSuccess);
            OnFileGenerated(filepath);
        }

        #region Events

        /// <summary>
        /// Occurs when a new data file is generated.
        /// </summary>
        public event EventHandler<FileGeneratedEventArgs> FileGenerated;

        public virtual void OnFileGenerated(string filepath) {
            var evt = FileGenerated;
            if (evt != null) {
                evt(this, new FileGeneratedEventArgs(filepath));
            }
        }

        #endregion

    }

}
