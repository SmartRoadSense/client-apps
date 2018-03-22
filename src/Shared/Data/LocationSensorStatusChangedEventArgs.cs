using System;

namespace SmartRoadSense.Shared.Data {

    public class LocationSensorStatusChangedEventArgs : EventArgs {

        public LocationSensorStatusChangedEventArgs(LocationSensorStatus previous, LocationSensorStatus current) {
            PreviousStatus = previous;
            CurrentStatus = current;
        }

        public LocationSensorStatus PreviousStatus { get; private set; }

        public LocationSensorStatus CurrentStatus { get; private set; }

    }

}

