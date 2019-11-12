using System;
using Xamarin.Essentials;

namespace SmartRoadSense {

    public static class Settings {

        private const string StartAtBootKey = "Preference.start_at_boot";
        private const string InstallationIdKey = "Preference.installation_id";
        private const string LastUploadAttemptKey = "Preference.last_upload_attempt";
        private const string LastVehicleTypeKey = "Preference.last_vehicle_type";
        private const string LastAnchorageTypeKey = "Preference.last_anchorage_type";
        private const string LastNumberOfPeopleKey = "Mnemonic.last_number_of_people";
        private const string DoNotAskForCarpoolingKey = "Preference.dont_ask_for_carpooling";
        private const string PreferUnmeteredConnectionKey = "Preference.prefer_unmetered_connection";
        private const string OfflineModeKey = "Preference.offline_mode";
        private const string DisableSuspensionKey = "Preference.disable_suspension";
        private const string DidShowTutorialKey = "Mnemonic.did_show_tutorial";
        private const string DataVersionKey = "Mnemonic.installed_data_version";
        private const string CalibrationDoneKey = "Calibration.done";
        private const string CalibrationScaleFactorKey = "Calibration.scale_factor";
        private const string CalibrationOriginalMagnitudeMeanKey = "Calibration.original_magnitude_mean";
        private const string CalibrationOriginalMagnitudeStdDevKey = "Calibration.original_magnitude_stddev";

        /// <summary>
        /// Gets or sets whether the application should start at boot.
        /// </summary>
        public static bool StartAtBoot {
            get {
                return Preferences.Get(StartAtBootKey, false);
            }
            set {
                Preferences.Set(StartAtBootKey, value);
            }
        }

        /// <summary>
        /// Gets the unique installation ID of the application.
        /// </summary>
        public static Guid InstallationId {
            get {
                string guidString = Preferences.Get(InstallationIdKey, (string)null);
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

                Preferences.Set(InstallationIdKey, id.ToString());

                return id;
            }
        }

        /// <summary>
        /// Gets or sets the last upload attempt.
        /// </summary>
        /// <value>Timestamp of the last upload attempt, using UTC.</value>
        public static DateTime LastUploadAttempt {
            get => Preferences.Get(LastUploadAttemptKey, DateTime.MinValue);
            set => Preferences.Set(LastUploadAttemptKey, value.ToUniversalTime());
        }

        public const VehicleType DefaultVehicleType = VehicleType.CAR;

        /// <summary>
        /// Gets or sets the last selected vehicle type.
        /// </summary>
        /// <value>The last type of the vehicle.</value>
        public static VehicleType LastVehicleType {
            get {
                var vehicleId = Preferences.Get(LastVehicleTypeKey, (int)DefaultVehicleType);

                if (Enum.IsDefined(typeof(VehicleType), vehicleId))
                    return (VehicleType)vehicleId;
                else
                    return DefaultVehicleType;
            }
            set => Preferences.Set(LastVehicleTypeKey, (int)value);
        }

        public const AnchorageType DefaultAnchorageType = AnchorageType.MOUNT;

        /// <summary>
        /// Gets or sets the last selected anchorage type.
        /// </summary>
        public static AnchorageType LastAnchorageType {
            get {
                var anchorageId = Preferences.Get(LastAnchorageTypeKey, (int)DefaultAnchorageType);

                if (Enum.IsDefined(typeof(AnchorageType), anchorageId))
                    return (AnchorageType)anchorageId;
                else
                    return DefaultAnchorageType;
            }
            set => Preferences.Set(LastAnchorageTypeKey, (int)value);
        }

        /// <summary>
        /// Gets or sets the last number of people traveling.
        /// </summary>
        public static int LastNumberOfPeople {
            get => Preferences.Get(LastNumberOfPeopleKey, -1);
            set => Preferences.Set(LastNumberOfPeopleKey, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool DoNotAskForCarpooling {
            get => Preferences.Get(DoNotAskForCarpoolingKey, false);
            set => Preferences.Set(DoNotAskForCarpoolingKey, value);
        }

        /// <summary>
        /// Gets or sets whether an unmetered (WiFi) connection should be preferred for uploading data.
        /// If set to false, any network will be used.
        /// </summary>
        public static bool PreferUnmeteredConnection {
            get => Preferences.Get(PreferUnmeteredConnectionKey, false);
            set => Preferences.Set(PreferUnmeteredConnectionKey, value);
        }

        /// <summary>
        /// Gets or sets whether the app is running in offline mode.
        /// </summary>
        public static bool OfflineMode {
            get => Preferences.Get(OfflineModeKey, false);
            set => Preferences.Set(OfflineModeKey, value);
        }

        /// <summary>
        /// Gets or sets whether suspension is disabled on timeout.
        /// </summary>
        public static bool SuspensionDisabled {
            get => Preferences.Get(DisableSuspensionKey, false);
            set => Preferences.Set(DisableSuspensionKey, value);
        }

        /// <summary>
        /// Gets or sets whether the start-up tutorial was shown and is completed.
        /// </summary>
        public static bool DidShowTutorial {
            get {
                var tutorial = Preferences.Get(DidShowTutorialKey, false);
                var calibration = Preferences.Get(CalibrationDoneKey, false);

                return tutorial && calibration;
            }
            set => Preferences.Set(DidShowTutorialKey, value);
        }

        public static int DataVersion {
            get => Preferences.Get(DataVersionKey, -1);
            set => Preferences.Set(DataVersionKey, value);
        }

        #region Calibration

        /// <summary>
        /// Gets or sets whether the calibration has been done.
        /// </summary>
        public static bool CalibrationDone {
            get => Preferences.Get(CalibrationDoneKey, false);
            set => Preferences.Set(CalibrationDoneKey, value);
        }

        /// <summary>
        /// Gets or sets the accelerometer scale factor determined during calibration.
        /// </summary>
        public static double CalibrationScaleFactor {
            get => Preferences.Get(CalibrationScaleFactorKey, 1.0);
            set => Preferences.Set(CalibrationScaleFactorKey, value);
        }

        /// <summary>
        /// Gets or sets the mean accelerometer magnitude detected during calibration.
        /// </summary>
        public static double CalibrationOriginalMagnitudeMean {
            get => Preferences.Get(CalibrationOriginalMagnitudeMeanKey, 0.0);
            set => Preferences.Set(CalibrationOriginalMagnitudeMeanKey, value);
        }

        /// <summary>
        /// Gets or sets the standard deviation of accelerometer magnitude detected during calibration.
        /// </summary>
        public static double CalibrationOriginalMagnitudeStdDev {
            get => Preferences.Get(CalibrationOriginalMagnitudeStdDevKey, 0.0);
            set => Preferences.Set(CalibrationOriginalMagnitudeStdDevKey, value);
        }

        #endregion Calibration

    }

}
