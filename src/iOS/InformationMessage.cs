using System;

namespace SmartRoadSense.iOS {

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

