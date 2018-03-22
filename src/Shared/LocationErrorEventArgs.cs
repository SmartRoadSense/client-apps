using System;

namespace SmartRoadSense.Shared {

    public class LocationErrorEventArgs : EventArgs {

        public LocationErrorEventArgs(LocationErrorType error) {
            Error = error;
        }

        public LocationErrorType Error { get; private set; }

    }

}
