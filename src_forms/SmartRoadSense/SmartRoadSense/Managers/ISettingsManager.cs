using System;
namespace SmartRoadSense
{
    public interface ISettingsManager
    {
        AnchorageType CurrentAnchorageType { get; set; }
        VehicleType CurrentVehicleType { get; set; }

        public bool StartAtBoot { get; set; }
        public Guid InstallationId { get; }
        public DateTime LastUploadAttempt { get; set; }
        public VehicleType LastVehicleType { get; set; }
        public AnchorageType LastAnchorageType { get; set; }
        public int LastNumberOfPeople { get; set; }
        public bool DoNotAskForCarpooling { get; set; }
        public bool PreferUnmeteredConnection { get; set; }
        public bool OfflineMode { get; set; }
        public bool SuspensionDisabled { get; set; }

        public bool DidShowTutorial { get; set; }
        public int DataVersion { get; set; }
        public bool CalibrationDone { get; set; }
        public double CalibrationScaleFactor { get; set; }
        public double CalibrationOriginalMagnitudeMean { get; set; }
        public double CalibrationOriginalMagnitudeStdDev { get; set; }

    }
}
