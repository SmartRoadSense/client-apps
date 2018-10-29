using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;

using SmartRoadSense.Shared;
using SmartRoadSense.Shared.Data;
using SmartRoadSense.Resources;

namespace SmartRoadSense.Shared.ViewModel {

    public class RecordingViewModel : BaseViewModel {

        private readonly SensorPack _sensors;
        private readonly Recorder _recorder;

        public RecordingViewModel() {
            _sensors = App.Sensors;
            _recorder = App.Recorder;

            //Setup commands
            StartRecordingCommand = new RelayCommand(HandleStartRecordingCommand);
            StopRecordingCommand = new RelayCommand(HandleStopRecordingCommand);
        }

        public override void OnCreate() {
            _sensors.LocationSensorStatusChanged += HandleLocationSensorStatusChanged;
            _sensors.LocationSensorError += HandleLocationSensorError;
            _sensors.InternalEngineErrorReported += HandleInternalEngineError;

            _recorder.DataPointRecorded += HandleDataPointRecorded;

            App.Sync.SyncError += HandleSyncError;
        }

        public override void OnDestroy() {
            _sensors.LocationSensorStatusChanged -= HandleLocationSensorStatusChanged;
			_sensors.LocationSensorError -= HandleLocationSensorError;
            _sensors.InternalEngineErrorReported -= HandleInternalEngineError;

            _recorder.DataPointRecorded -= HandleDataPointRecorded;

            App.Sync.SyncError -= HandleSyncError;
        }

        private void HandleLocationSensorStatusChanged(object sender, LocationSensorStatusChangedEventArgs e) {
            if (e.PreviousStatus == LocationSensorStatus.Fixing && e.CurrentStatus == LocationSensorStatus.Working) {
                UserLog.Add(UserLog.Icon.None, LogStrings.GpsFix);
            }
            else if (e.PreviousStatus == LocationSensorStatus.Working && e.CurrentStatus == LocationSensorStatus.Fixing) {
                UserLog.Add(UserLog.Icon.None, LogStrings.GpsFixLost);
            }
            else if (e.CurrentStatus == LocationSensorStatus.Disabled) {
                UserLog.Add(UserLog.Icon.None, LogStrings.GpsDisabled);
            }

            OnPropertyChanged(() => LocationSensorStatus);
            SensorStatusUpdated.Raise(this);
        }

        private void HandleLocationSensorError(object sender, LocationErrorEventArgs e) {
            switch(e.Error) {
            case LocationErrorType.RemainedStationary:
                Log.Debug("GPS stationary for too long: stopping recording");
                UserLog.Add(UserLog.Icon.Warning, LogStrings.RecordingSuspendedStationary);
                break;

            case LocationErrorType.SpeedTooLow:
                Log.Debug("User moving too slowly: stopping recording");
                UserLog.Add(UserLog.Icon.Warning, LogStrings.RecordingSuspendedSpeed);
                break;
            }

            RecordingSuspended.Raise(this, e);

            StopRecordingCommand.Execute(null);
        }

        private void HandleInternalEngineError(object sender, InternalEngineErrorEventArgs e) {
            Log.Debug("Handling internal engine error");
            UserLog.Add(UserLog.Icon.Error, LogStrings.InternalEngineError);

#if !WINDOWS_PHONE_APP
            //Dump will be performed in the background while UI goes on
            ErrorReporter.ExecuteDump(e.Exception).Forget();
#endif

            InternalEngineErrorReported.Raise(this);

            StopRecordingCommand.Execute(null);
        }

        private void HandleDataPointRecorded(object sender, DataPointRecordedEventArgs e) {
            OnPropertyChanged(() => CurrentPpe);
            OnPropertyChanged(() => MinimumPpe);
            OnPropertyChanged(() => MaximumPpe);
            MeasurementsUpdated.Raise(this);
        }

        private void HandleSyncError(object sender, SyncErrorEventArgs e) {
            SyncErrorReported.Raise(this, e);
        }

#region Bindable properties

        public bool IsRecording {
            get {
                return _recorder.IsRecording;
            }
        }

        public double CurrentPpe {
            get {
                return _recorder.Session.LastMeasurement;
            }
        }

        public double MinimumPpe {
            get {
                return _recorder.Session.MinimumMeasurement;
            }
        }

        public double MaximumPpe {
            get {
                return _recorder.Session.MaximumMeasurement;
            }
        }

        public VehicleType CurrentVehicleType {
            get {
                return _recorder.Session.Vehicle;
            }
        }

        public AnchorageType CurrentAnchorageType {
            get {
                return _recorder.Session.Anchorage;
            }
        }

        public Guid TrackId {
            get {
                return _recorder.Session.TrackId;
            }
        }

        public LocationSensorStatus LocationSensorStatus {
            get {
                return _sensors.LocationSensorStatus;
            }
        }

        /// <summary>
        /// Gets whether the SmartRoadSense engine is currently working correctly and is reporting PPE values.
        /// </summary>
        /// <remarks>
        /// True is the sensors are online (GPS is fixed) and at least a displayable PPE value is available
        /// to be shown.
        /// </remarks>
        public bool IsReporting {
            get {
                return (
                    IsRecording &&
					_sensors.LocationSensorStatus.IsActive() &&
                    !double.IsNaN(_recorder.Session.LastMeasurement)
                );
            }
        }

#endregion

#region UI Commands

        public ICommand StartRecordingCommand { get; private set; }

        private void HandleStartRecordingCommand() {
            if (_recorder.IsRecording) {
                Log.Debug("Ignoring start recording command since recorder already running");
                return;
            }

			if(!Settings.CalibrationDone) {
				Log.Debug("Ignoring start recording command since device is not calibrated");
				return;
			}

            Log.Debug("Executing start recording command");

            _sensors.StartSensing();
            _recorder.Start();

            //Ensure GPS information is refreshed before signaling recording
            OnPropertyChanged(() => LocationSensorStatus);
            SensorStatusUpdated.Raise(this);

            OnPropertyChanged(() => IsRecording);
            RecordingStatusUpdated.Raise(this);

            //Starting a new recording regenerates session information
            OnPropertyChanged(() => CurrentVehicleType);
            OnPropertyChanged(() => CurrentAnchorageType);
            OnPropertyChanged(() => TrackId);
        }

        public ICommand StopRecordingCommand { get; private set; }

        private void HandleStopRecordingCommand() {
            Log.Debug("Executing stop recording command");

            _sensors.StopSensing();
            _recorder.Stop();

            OnPropertyChanged(() => IsRecording);

            RecordingStatusUpdated.Raise(this);
        }

#endregion

#region Manual event binding (Android + iOS)

        /// <summary>
        /// Occurs when measurements are updated.
        /// </summary>
        public event EventHandler MeasurementsUpdated;

        /// <summary>
        /// Occurs when the recording status is updated.
        /// </summary>
        public event EventHandler RecordingStatusUpdated;

        /// <summary>
        /// Occurs when the sensors' status is updated.
        /// </summary>
        public event EventHandler SensorStatusUpdated;

        /// <summary>
        /// Occurs when the recording is suspended because of a location sensor error.
        /// </summary>
		public event EventHandler<LocationErrorEventArgs> RecordingSuspended;

        /// <summary>
        /// Occurs when an internal engine error was reported and handled.
        /// </summary>
        public event EventHandler InternalEngineErrorReported;

        /// <summary>
        /// Occurs when a synchronization error occurs.
        /// </summary>
        public event EventHandler<SyncErrorEventArgs> SyncErrorReported;

#endregion

    }

}

