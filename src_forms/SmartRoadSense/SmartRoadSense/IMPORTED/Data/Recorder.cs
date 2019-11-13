using System;
using SmartRoadSense.Core;

namespace SmartRoadSense {

    public class Recorder {
        private bool _isRecording;

        public Recorder(Engine engine) {
            Collector = new DataCollector();
            StatsCollector = new StatisticsCollector();
            Collector.FileGenerated += HandleCollectorFileGenerated;

            Engine = engine;
            Engine.ComputationCompleted += HandleEngineComputationCompleted;

            Log.Debug("Recorder initialized");
        }

        /// <summary>
        /// Gets the data collector used by the recorder.
        /// </summary>
        public DataCollector Collector { get; }

        /// <summary>
        /// Gets the statistic collector used by the recorder.
        /// </summary>
        public StatisticsCollector StatsCollector { get; }

        /// <summary>
        /// Gets the engine used by the recorder.
        /// </summary>
        public Engine Engine { get; }

        /// <summary>
        /// Gets the current session information.
        /// </summary>
        public SessionInfo Session { get; private set; }

        private void HandleEngineComputationCompleted(object sender, EngineComputationEventArgs e) {
            Session.NewMeasurement(e.Result.Ppe);

            if (_isRecording) {
                var dataPiece = new DataPiece {
                    TrackId = Session.TrackId,
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
                    Vehicle = Session.Vehicle,
                    Anchorage = Session.Anchorage,
                    NumberOfPeople = Session.NumberOfPeople
                };

                if (!SettingsManager.Instance.OfflineMode) {
                    Collector.Collect(dataPiece);
                    StatsCollector.Collect(dataPiece);
                }

                OnDataPointRecorded(dataPiece, e.Result);
            }
        }

        private void HandleCollectorFileGenerated(object sender, FileGeneratedEventArgs e) {
            OnDataFileWritten(e);
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
            if(!SettingsManager.Instance.CalibrationDone) {
                throw new InvalidOperationException("Cannot start recording before completing accelerometer calibration");
            }

            Session = new SessionInfo(SettingsManager.Instance.LastVehicleType, SettingsManager.Instance.LastAnchorageType, SettingsManager.Instance.LastNumberOfPeople);
            _isRecording = true;

            if (SettingsManager.Instance.OfflineMode) {
                UserLog.Add(AppResources.RecordingOffline);
                Log.Debug("Started offline recording");
            }
            else {
                UserLog.Add(AppResources.RecordingStarted);
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
            Collector.Flush();
            StatsCollector.CompleteSession();

            UserLog.Add(AppResources.RecordingStopped);

            Log.Debug("Stopped recording");
        }

        #region Events

        /// <summary>
        /// Occurs when a new data point is recorded.
        /// </summary>
        public event EventHandler<DataPointRecordedEventArgs> DataPointRecorded;

        protected virtual void OnDataPointRecorded(DataPiece data, Result result) {
            DataPointRecorded?.Invoke(this, new DataPointRecordedEventArgs(data, Session, result));
            //var evt = DataPointRecorded;
            //if (evt != null)
            //{
            //    evt(this, new DataPointRecordedEventArgs(data, _sessionInfo, result));
            //}
        }

        /// <summary>
        /// Occurs when a new data file is written to disk.
        /// </summary>
        public event EventHandler<FileGeneratedEventArgs> DataFileWritten;

        protected virtual void OnDataFileWritten(FileGeneratedEventArgs args) {
            //var evt = DataFileWritten;
            //if (evt != null) {
            //    evt(this, args);
            //}
            DataFileWritten?.Invoke(this, args);
        }

        #endregion

    }

}

