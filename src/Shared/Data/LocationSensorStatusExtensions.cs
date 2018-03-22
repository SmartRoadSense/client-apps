using System;

namespace SmartRoadSense.Shared.Data {

    public static class LocationSensorStatusExtensions {

        /// <summary>
        /// Determines whether the status indicates an active location sensor.
        /// A location sensor is deemed to be active when it is currently reporting the device's
        /// location.
        /// </summary>
        public static bool IsActive(this LocationSensorStatus status) {
            return (status == LocationSensorStatus.Working || status == LocationSensorStatus.OutOfCountry);
        }

    }

}

