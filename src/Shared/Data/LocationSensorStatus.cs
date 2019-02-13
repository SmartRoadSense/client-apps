using System;

namespace SmartRoadSense.Shared.Data {

    /// <summary>
    /// Describes the status of the location sensor.
    /// </summary>
    public enum LocationSensorStatus {
        /// <summary>
        /// The sensor is collecting data.
        /// </summary>
        Working,
        /// <summary>
        /// The sensor is attempting to fix.
        /// </summary>
        Fixing,
        /// <summary>
        /// The sensor is disabled.
        /// </summary>
        Disabled,
        /// <summary>
        /// The sensor is working, but the location reported is in a
        /// country SmartRoadSense does not support.
        /// </summary>
        OutOfCountry,
    }

}

