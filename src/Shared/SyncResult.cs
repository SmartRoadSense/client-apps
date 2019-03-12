using System;

namespace SmartRoadSense.Shared {

    public class SyncResult {

        public SyncResult(Exception error) {
            Error = error;
        }

        public SyncResult(int points, int chunks) {
            PointsUploaded = points;
            ChunksUploaded = chunks;
        }

        public readonly int PointsUploaded;

        public readonly int ChunksUploaded;

        public readonly Exception Error;

        public bool HasFailed {
            get {
                return Error != null;
            }
        }

    }

}

