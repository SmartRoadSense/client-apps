using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace SmartRoadSense.Shared {

    public static class Settings {

        private static ISettings InternalSettings {
            get {
                return CrossSettings.Current;
            }
        }

#if __ANDROID__

        private const string StartAtBootKey = "Preference.start_at_boot";

        /// <summary>
        /// Gets or sets whether the application should start at boot.
        /// </summary>
        public static bool StartAtBoot {
            get {
                return InternalSettings.GetValueOrDefault(StartAtBootKey, false);
            }
            set {
                InternalSettings.AddOrUpdateValue(StartAtBootKey, value);
            }
        }

#endif

        private const string InstallationIdKey = "Preference.installation_id";

        /// <summary>
        /// Gets the unique installation ID of the application.
        /// </summary>
        public static Guid InstallationId {
            get {
                string guidString = InternalSettings.GetValueOrDefault(InstallationIdKey, (string)null);
                if (!string.IsNullOrWhiteSpace(guidString)) {
                    if(Guid.TryParse(guidString, out Guid ret)) {
                        return ret;
                    }
                    else {
                        Log.Warning(new ArgumentException(nameof(guidString)), "Invalid installation ID in settings '{0}'", guidString);
                    }
                }

                Guid id = Guid.NewGuid();
                Log.Debug("Generating new installation ID ({0:D})", id);

                InternalSettings.AddOrUpdateValue(InstallationIdKey, id.ToString());

                return id;
            }
        }

        private const string LastUploadAttemptKey = "Preference.last_upload_attempt";

        /// <summary>
        /// Gets or sets the last upload attempt.
        /// </summary>
        /// <value>Timestamp of the last upload attempt, using UTC.</value>
        public static DateTime LastUploadAttempt {
            get {
                return InternalSettings.GetValueOrDefault(LastUploadAttemptKey, DateTime.MinValue);
            }
            set {
                InternalSettings.AddOrUpdateValue(LastUploadAttemptKey, value.ToUniversalTime());
            }
        }

        private const string LastVehicleTypeKey = "Preference.last_vehicle_type";

        public const VehicleType DefaultVehicleType = VehicleType.Car;

        /// <summary>
        /// Gets or sets the last selected vehicle type.
        /// </summary>
        /// <value>The last type of the vehicle.</value>
        public static VehicleType LastVehicleType {
            get {
                var vehicleId = InternalSettings.GetValueOrDefault(LastVehicleTypeKey, (int)DefaultVehicleType);

                if (Enum.IsDefined(typeof(VehicleType), vehicleId))
                    return (VehicleType)vehicleId;
                else
                    return DefaultVehicleType;
            }
            set {
                InternalSettings.AddOrUpdateValue(LastVehicleTypeKey, (int)value);
            }
        }

        private const string LastAnchorageTypeKey = "Preference.last_anchorage_type";

        public const AnchorageType DefaultAnchorageType = AnchorageType.MobileBracket;

        /// <summary>
        /// Gets or sets the last selected anchorage type.
        /// </summary>
        public static AnchorageType LastAnchorageType {
            get {
                var anchorageId = InternalSettings.GetValueOrDefault(LastAnchorageTypeKey, (int)DefaultAnchorageType);

                if (Enum.IsDefined(typeof(AnchorageType), anchorageId))
                    return (AnchorageType)anchorageId;
                else
                    return DefaultAnchorageType;
            }
            set {
                InternalSettings.AddOrUpdateValue(LastAnchorageTypeKey, (int)value);
            }
        }

        private const string LastNumberOfPeopleKey = "Mnemonic.last_number_of_people";

        public const int DefaultNumberOfPeople = -1;

        /// <summary>
        /// Gets or sets the last number of people traveling.
        /// </summary>
        public static int LastNumberOfPeople {
            get {
                return InternalSettings.GetValueOrDefault(LastNumberOfPeopleKey, DefaultNumberOfPeople);
            }
            set {
                InternalSettings.AddOrUpdateValue(LastNumberOfPeopleKey, value);
            }
        }

        private const string DoNotAskForCarpoolingKey = "Preference.dont_ask_for_carpooling";

        public static bool DoNotAskForCarpooling {
            get => InternalSettings.GetValueOrDefault(DoNotAskForCarpoolingKey, false);
            set => InternalSettings.AddOrUpdateValue(DoNotAskForCarpoolingKey, value);
        }

        private const string PreferUnmeteredConnectionKey = "Preference.prefer_unmetered_connection";

        /// <summary>
        /// Gets or sets whether an unmetered (WiFi) connection should be preferred for uploading data.
        /// If set to false, any network will be used.
        /// </summary>
        public static bool PreferUnmeteredConnection {
            get {
                return InternalSettings.GetValueOrDefault(PreferUnmeteredConnectionKey, false);
            }
            set {
                InternalSettings.AddOrUpdateValue(PreferUnmeteredConnectionKey, value);
            }
        }

        private const string OfflineModeKey = "Preference.offline_mode";

        /// <summary>
        /// Gets or sets whether the app is running in offline mode.
        /// </summary>
        public static bool OfflineMode {
            get {
                return InternalSettings.GetValueOrDefault(OfflineModeKey, false);
            }
            set {
                InternalSettings.AddOrUpdateValue(OfflineModeKey, value);
            }
        }

        private const string DisableSuspensionKey = "Preference.disable_suspension";

        /// <summary>
        /// Gets or sets whether suspension is disabled on timeout.
        /// </summary>
        public static bool SuspensionDisabled {
            get {
                return InternalSettings.GetValueOrDefault(DisableSuspensionKey, false);
            }
            set {
                InternalSettings.AddOrUpdateValue(DisableSuspensionKey, value);
            }
        }

        private const string DidShowTutorialKey = "Mnemonic.did_show_tutorial";

        /// <summary>
        /// Gets or sets whether the start-up tutorial was shown and is completed.
        /// </summary>
        public static bool DidShowTutorial {
            get {
                var tutorial = InternalSettings.GetValueOrDefault(DidShowTutorialKey, false);
                var calibration = InternalSettings.GetValueOrDefault(CalibrationDoneKey, false);

                return tutorial && calibration;
            }
            set {
                InternalSettings.AddOrUpdateValue(DidShowTutorialKey, value);
            }
        }

        private const string DataVersionKey = "Mnemonic.installed_data_version";

        public static int DataVersion {
            get {
                return InternalSettings.GetValueOrDefault(DataVersionKey, -1);
            }
            set {
                InternalSettings.AddOrUpdateValue(DataVersionKey, value);
            }
        }

        #region Calibration

        private const string CalibrationDoneKey = "Calibration.done";

        /// <summary>
        /// Gets or sets whether the calibration has been done.
        /// </summary>
        public static bool CalibrationDone {
            get {
                return InternalSettings.GetValueOrDefault(CalibrationDoneKey, false);
            }
            set {
                InternalSettings.AddOrUpdateValue(CalibrationDoneKey, value);
            }
        }

        private const string CalibrationScaleFactorKey = "Calibration.scale_factor";

        /// <summary>
        /// Gets or sets the accelerometer scale factor determined during calibration.
        /// </summary>
        public static double CalibrationScaleFactor {
            get {
                return InternalSettings.GetValueOrDefault(CalibrationScaleFactorKey, 1.0);
            }
            set {
                InternalSettings.AddOrUpdateValue(CalibrationScaleFactorKey, value);
            }
        }

        private const string CalibrationOriginalMagnitudeMeanKey = "Calibration.original_magnitude_mean";

        /// <summary>
        /// Gets or sets the mean accelerometer magnitude detected during calibration.
        /// </summary>
        public static double CalibrationOriginalMagnitudeMean {
            get {
                return InternalSettings.GetValueOrDefault(CalibrationOriginalMagnitudeMeanKey, 0.0);
            }
            set {
                InternalSettings.AddOrUpdateValue(CalibrationOriginalMagnitudeMeanKey, value);
            }
        }

        private const string CalibrationOriginalMagnitudeStdDevKey = "Calibration.original_magnitude_stddev";

        /// <summary>
        /// Gets or sets the standard deviation of accelerometer magnitude detected during calibration.
        /// </summary>
        public static double CalibrationOriginalMagnitudeStdDev {
            get {
                return InternalSettings.GetValueOrDefault(CalibrationOriginalMagnitudeStdDevKey, 0.0);
            }
            set {
                InternalSettings.AddOrUpdateValue(CalibrationOriginalMagnitudeStdDevKey, value);
            }
        }

        #endregion Calibration

    }

}
