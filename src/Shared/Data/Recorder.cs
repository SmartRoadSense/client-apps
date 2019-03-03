using System;
using System.Collections.Generic;

using SmartRoadSense.Core;
using SmartRoadSense.Resources;
using SmartRoadSense.Shared.Database;

namespace SmartRoadSense.Shared.Data {

    public class Recorder {

        /// <summary>
        /// Maximum pause interval between data points.
        /// If exceeded, the recorder will automatically start a new session.
        /// </summary>
#if DEBUG
        public static readonly TimeSpan MaximumPause = TimeSpan.FromMinutes(10);
#else
        public static readonly TimeSpan MaximumPause = TimeSpan.FromHours(1);
#endif

        private readonly DataWriter _statsCollector;
        private readonly Engine _engine;

        private SessionInfo _sessionInfo;

        private bool _isRecording = false;

        public Recorder(Engine engine) {
            _statsCollector = new DataWriter();

            _engine = engine;
            _engine.ComputationCompleted += HandleEngineComputationCompleted;

            Log.Debug("Recorder initialized");
        }

        /// <summary>
        /// Gets the statistic collector used by the recorder.
        /// </summary>
        public DataWriter StatsCollector {
            get {
                return _statsCollector;
            }
        }

        /// <summary>
        /// Gets the engine used by the recorder.
        /// </summary>
        public Engine Engine {
            get {
                return _engine;
            }
        }

        /// <summary>
        /// Gets the current session information.
        /// </summary>
        public SessionInfo Session {
            get {
                return _sessionInfo;
            }
        }

        private DateTime? _lastMeasurementCollection = null;

        private void HandleEngineComputationCompleted(object sender, EngineComputationEventArgs e) {
            _sessionInfo.NewMeasurement(e.Result.Ppe);

            if (_isRecording) {
                if(_lastMeasurementCollection.HasValue) {
                    if(DateTime.UtcNow - _lastMeasurementCollection.Value > MaximumPause) {
                        Log.Debug("Exceeded maximum interval between recording, restarting");

                        Stop();
                        Start();

                        Log.Event("Recorder.IntervalExceeded");
                    }
                }

                var dataPiece = new DataPiece {
                    TrackId = _sessionInfo.TrackId,
                    StartTimestamp = new DateTime(e.Result.FirstTimestamp, DateTimeKind.Utc),
                    EndTimestamp = new DateTime(e.Result.LastTimestamp, DateTimeKind.Utc),
                    Ppe = e.Result.Ppe,
                    PpeX = e.Result.PpeX,
                    PpeY = e.Result.PpeY,
                    PpeZ = e.Result.PpeZ,
                    Latitude = e.Result.Latitude,
                    Longitude = e.Result.Longitude,
                    Bearing = e.Result.Bearing,
                    Accuracy = e.Result.Accuracy,
                    Vehicle = _sessionInfo.Vehicle,
                    Anchorage = _sessionInfo.Anchorage,
                    NumberOfPeople = _sessionInfo.NumberOfPeople
                };

                if (!Settings.OfflineMode) {
                    _statsCollector.Collect(dataPiece);
                }

                OnDataPointRecorded(dataPiece, e.Result);
                _lastMeasurementCollection = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Gets or sets whether the recorder is currently recording.
        /// </summary>
        public bool IsRecording {
            get {
                return _isRecording;
            }
            set {
                if (_isRecording != value) {
                    if (value)
                        Start();
                    else
                        Stop();
                }
            }
        }

        /// <summary>
        /// Starts recording.
        /// </summary>
        public void Start() {
            if (_isRecording) {
                Log.Debug("Already recording");
                return;
            }

            //Prevent recording before completing calibration
            if(!Settings.CalibrationDone) {
                throw new InvalidOperationException("Cannot start recording before completing accelerometer calibration");
            }

            _sessionInfo = new SessionInfo(Settings.LastVehicleType, Settings.LastAnchorageType, Settings.LastNumberOfPeople);
            _isRecording = true;

            if (Settings.OfflineMode) {
                UserLog.Add(LogStrings.RecordingOffline);
                Log.Debug("Started offline recording");
            }
            else {
                UserLog.Add(LogStrings.RecordingStarted);
                Log.Debug("Started recording");
            }
        }

        /// <summary>
        /// Stops recording.
        /// </summary>
        public void Stop() {
            if (!_isRecording) {
                Log.Debug("Already stopped");
                return;
            }

            _isRecording = false;
            _lastMeasurementCollection = null;
            _statsCollector.CompleteSession();

            UserLog.Add(LogStrings.RecordingStopped);

            Log.Debug("Stopped recording");
        }

#region Events

        /// <summary>
        /// Occurs when a new data point is recorded.
        /// </summary>
        public event EventHandler<DataPointRecordedEventArgs> DataPointRecorded;

        protected virtual void OnDataPointRecorded(DataPiece data, Result result) {
            DataPointRecorded?.Invoke(this, new DataPointRecordedEventArgs(data, _sessionInfo, result));
        }

#endregion

    }

}

