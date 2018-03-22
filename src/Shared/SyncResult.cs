using System;

namespace SmartRoadSense.Shared {

    public class SyncResult {

        public SyncResult(
            int dataPiecesUploaded = 0,
            int dataPiecesDeleted = 0,
            Exception error = null
        ) {
            DataPiecesUploaded = dataPiecesUploaded;
            DataPiecesDeleted = dataPiecesDeleted;
            Error = error;
        }

        public readonly int DataPiecesUploaded;

        public readonly int DataPiecesDeleted;

        public readonly Exception Error;

        public bool HasFailed {
            get {
                return Error != null;
            }
        }

    }

}

