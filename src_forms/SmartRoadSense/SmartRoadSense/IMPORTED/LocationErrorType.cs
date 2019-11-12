using System;

namespace SmartRoadSense {

    /// <summary>
    /// Describes an issue with the location sensor/tracking.
    /// </summary>
    public enum LocationErrorType {
        /// <summary>
        /// The location reports a low speed that cannot be used.
        /// </summary>
        SpeedTooLow,
        /// <summary>
        /// The location remained stationary for some time.
        /// </summary>
        RemainedStationary
    }

}

