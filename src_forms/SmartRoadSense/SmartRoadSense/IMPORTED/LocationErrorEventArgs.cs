using System;

namespace SmartRoadSense {

    public class LocationErrorEventArgs : EventArgs {

        public LocationErrorEventArgs(LocationErrorType error) {
            Error = error;
        }

        public LocationErrorType Error { get; private set; }

    }

}
