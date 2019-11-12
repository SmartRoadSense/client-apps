using System;

namespace SmartRoadSense {

    public class SyncErrorEventArgs : EventArgs {

        public SyncErrorEventArgs(Exception error) {
            Error = error;
        }

        public Exception Error { get; private set; }

    }

}

