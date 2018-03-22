using System;

namespace SmartRoadSense.Android {

    /// <summary>
    /// Information messages that can be displayed on main recording UI.
    /// </summary>
    public enum InformationMessage {
        GpsDisabled,
        GpsUnfixed,
        UploadFailure,
        GpsSuspendedStationary,
		GpsSuspendedSpeed,
        OutOfCountry,
        InternalEngineError,
        Syncing
    }

}

