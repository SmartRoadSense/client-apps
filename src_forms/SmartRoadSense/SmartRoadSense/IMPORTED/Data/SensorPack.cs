using System;
using SmartRoadSense.Core;
using static SmartRoadSense.Log;

namespace SmartRoadSense {

    /// <summary>
    /// Sensor pack around the algorithmic engine.
    /// </summary>
    /// <remarks>
    /// This class derives from Java.Lang.Object because of Xamarin limitations on Android.
    /// </remarks>
//#if __ANDROID__
//    public abstract class SensorPack : Java.Lang.Object {
//#else
    public abstract class SensorPack {
//#endif

        public const int AccelerometerDesiredHz = 100;
        public const int AccelerometerDesiredDelayMs = 1000 / AccelerometerDesiredHz;

        public const int LocationDesiredHz = 1;
        public const int LocationDesiredDelayMs = 1000 / LocationDesiredHz;

        /// <summary>
        /// Timeout before signaling errors because of unmoving or too slow location updates.
        /// </summary>
#if DEBUG
        private readonly TimeSpan GpsDistanceErrorTimeout = TimeSpan.FromMinutes(2);
#else
        private readonly TimeSpan GpsDistanceErrorTimeout = TimeSpan.FromMinutes(6);
#endif

        /// <summary>
        /// Timeout since the last GPS update before the sensor is considered to be unfixed.
        /// </summary>
        private readonly TimeSpan GpsUnfixedTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Minimum speed (in m/s) to consider the user to be moving at all.
        /// </summary>
        /// <remarks>
        /// 1.5 m/s, approx. 5 km/h, considered to be an acceptable "movement" speed.
        /// Lesser distances between GPS updates are usually due to GPS inaccuracy.
        /// </remarks>
#if !DEBUG
        private const double GpsUnmovingSpeed = 1.5;
#else
        private const double GpsUnmovingSpeed = 0.0;
#endif
        /// <summary>
        /// Minimum speed (in m/s) to consider the user to be fast enough.
        /// </summary>
        /// <remarks>
        /// 20 km/h ~= 5.5 m/s.
        /// </remarks>
#if !DEBUG
        private const double GpsFastEnoughSpeed = 20 * 1000 / 3600;
#else
        private const double GpsFastEnoughSpeed = 0.0;
#endif

        /// <summary>
        /// Maximum speed for location updates to be considered too far away between each other.
        /// </summary>
        /// <remarks>
        /// 300 m/s, i.e. 200 km/h, speeds assumed to be related to GPS adjusting or some other spurious skip.
        /// Computation and GPS should be reset after a long unforeseen skip.
        /// </remarks>
        private const double GpsTooFastSpeed = 300;

        private readonly Engine _engine;

        private bool _isActive = false;

        protected SensorPack(Engine engine) {
            _engine = engine;
        }

        /// <summary>
        /// SensorPack factory method.
        /// </summary>
        public static SensorPack Create(Engine engine) {
            return new SensorPackImplementation(engine);
//#if __ANDROID__
//            return new SensorPackAndroid(engine);
//#elif __IOS__
//            return new SensorPackIOS(engine);
//#elif DESKTOP
//            return new SensorPackFake(engine);
//#else
//            return new SensorPackFake(engine);
//#endif
        }

        private LocationSensorStatus _locationSensorStatus = LocationSensorStatus.Fixing;

        /// <summary>
        /// Gets the current status of the location sensor.
        /// </summary>
        public LocationSensorStatus LocationSensorStatus {
            get {
                if (Settings.OfflineMode) {
                    //In offline mode the location sensor is always dandy
                    return LocationSensorStatus.Working;
                }

                return _locationSensorStatus;
            }
            protected set {
                //Once signaled as "out of country", the GPS stays there when switching to working
                //TODO: this isn't actually implemented
                if (_locationSensorStatus == LocationSensorStatus.OutOfCountry && value == LocationSensorStatus.Working)
                    return;

                //IF the sensor started working now, reset timeout timers
                if (_locationSensorStatus != LocationSensorStatus.Working && value == LocationSensorStatus.Working) {
                    _lastTimeFastEnough = DateTime.UtcNow;
                    _lastTimeMoved = DateTime.UtcNow;
                }

                if (_locationSensorStatus != value) {
                    Debug("Sensor status {0} -> {1}", _locationSensorStatus, value);

                    var oldStatus = _locationSensorStatus;
                    _locationSensorStatus = value;

                    OnLocationSensorStatusChanged(oldStatus, value);
                }
            }
        }

        public event EventHandler<LocationSensorStatusChangedEventArgs> LocationSensorStatusChanged;

        protected virtual void OnLocationSensorStatusChanged(LocationSensorStatus previous, LocationSensorStatus current) {
            LocationSensorStatusChanged?.Invoke(this, new LocationSensorStatusChangedEventArgs(previous, current));
        }

        public event EventHandler<LocationErrorEventArgs> LocationSensorError;

        protected virtual void OnLocationSensorError(LocationErrorType error) {
            ResetGpsLocation();

            LocationSensorError?.Invoke(this, new LocationErrorEventArgs(error));
        }

        public event EventHandler<InternalEngineErrorEventArgs> InternalEngineErrorReported;

        protected virtual void OnInternalEngineErrorReported(EngineComputationException exception) {
            InternalEngineErrorReported?.Invoke(this, new InternalEngineErrorEventArgs(exception));
        }

        private bool _isMovingSlowly = true;

        /// <summary>
        /// Gets whether the user is currently moving slowly, as detected by the location sensors.
        /// </summary>
        /// <remarks>
        /// When this value is true, the location sensors have detected a movement below a certain preset speed.
        /// Current user movement is evalued at each location sensor update (i.e., approximately once per second).
        /// If the user is slow, the values from the engine can be ignored.
        /// This value defaults to true, in order to wait for the user to cross the speed threshold and activate
        /// recording.
        /// </remarks>
        public bool IsMovingSlowly {
            get {
                return _isMovingSlowly;
            }
            private set {
                if (_isMovingSlowly != value) {
                    _isMovingSlowly = value;

                    if (value) {
                        StartedMovingSlowly.Raise(this);
                    }
                    else {
                        StoppedMovingSlowly.Raise(this);
                    }
                }
            }
        }

        /// <summary>
        /// Raised when the user starts moving slowly.
        /// </summary>
        public event EventHandler StartedMovingSlowly;

        /// <summary>
        /// Raised when the user stops moving slowly.
        /// </summary>
        public event EventHandler StoppedMovingSlowly;

#region Common sensor data handling

        private DateTime _gpsLastUpdate;

        private DateTime _lastTimeMoved;
        private DateTime _lastTimeFastEnough;

        private double _gpsLastLatitude = double.NaN;
        private double _gpsLastLongitude = double.NaN;
        private float _gpsLastSpeed = float.NaN;
        private float _gpsLastBearing;
        private int _gpsLastAccuracy;

        /// <summary>
        /// Minimum required accuracy for GPS location (in meters).
        /// </summary>
        public const int MinimumAccuracy = 15;

        /// <summary>
        /// Reports a new location value from the GPS sensor.
        /// </summary>
        /// <param name="latitude">Latitude in degrees.</param>
        /// <param name="longitude">Longitude in degrees.</param>
        /// <param name="speed">Speed in m/s.</param>
        /// <param name="bearing">Bearing in degrees.</param>
        /// <param name="accuracy">Accuracy in meters.</param>
        protected void ReportNewLocation(double latitude, double longitude, float speed, float bearing, int accuracy) {
            LocationSensorStatus = LocationSensorStatus.Working;

            var timestamp = DateTime.UtcNow;
            var intervalTime = timestamp - _gpsLastUpdate;
            _gpsLastUpdate = timestamp;
            Debug("Location ({0:F3};{1:F3}) at {2:F2} m/s, accuracy {3:F0} m, elapsed {4:F1} ms",
                latitude, longitude, speed, accuracy, intervalTime.TotalMilliseconds);

            if(accuracy > MinimumAccuracy) {
                Debug("Location too inaccurate, discarding");
                return;
            }

            //TODO: add location interpolation here

            if (_gpsLastLatitude.IsValid() && _gpsLastLongitude.IsValid()) {
                //We use very naïve measurements here
                var measuredDistance = GeoHelper.DistanceBetweenPoints(_gpsLastLatitude, _gpsLastLongitude, latitude, longitude) * 1000.0; // m
                var measuredSpeed = measuredDistance / intervalTime.TotalSeconds; // m/s
                Debug("Traveled {0:F5} m, at {1:F1} m/s: {2}",
                    measuredDistance, measuredSpeed,
                    (measuredSpeed > GpsUnmovingSpeed) ? ((measuredSpeed > GpsFastEnoughSpeed) ? ((measuredSpeed > GpsTooFastSpeed) ? "too fast" : "OK") : "slow") : "unmoving"
                );

                _gpsLastSpeed = (float)measuredSpeed;

                if (measuredSpeed >= GpsUnmovingSpeed) {
                    _lastTimeMoved = timestamp;

                    if (measuredSpeed >= GpsFastEnoughSpeed) {
                        _lastTimeFastEnough = timestamp;
                        IsMovingSlowly = false;

                        if (measuredSpeed >= GpsTooFastSpeed) {
                            Warning(new ArgumentOutOfRangeException(nameof(measuredSpeed)), "Unforeseen GPS skip of {0} m in {1} ms", measuredDistance, intervalTime.TotalMilliseconds);
                            ResetGpsLocation();
                            return;
                        }
                    }
                    else {
                        IsMovingSlowly = true;
                    }
                }
            }

            _gpsLastLatitude = latitude;
            _gpsLastLongitude = longitude;
            _gpsLastBearing = bearing;
            _gpsLastAccuracy = accuracy;
        }

        /// <summary>
        /// Reports new acceleration values from the sensor.
        /// </summary>
        /// <param name="accX">X-axis acceleration in m/s^2.</param>
        /// <param name="accY">Y-axis acceleration in m/s^2.</param>
        /// <param name="accZ">Z-axis acceleration in m/s^2.</param>
        protected void ReportNewAcceleration(double accX, double accY, double accZ) {
            //Scale each axis individually
            accX *= _accelerometerScaleFactor;
            accY *= _accelerometerScaleFactor;
            accZ *= _accelerometerScaleFactor;

            //In offline mode we ignore GPS status and simply feed fake data to the engine
            if (Settings.OfflineMode) {
                FeedEngine(new DataEntry(
                    DateTime.UtcNow,
                    accX, accY, accZ,
                    0.0, 0.0,
                    0.0f,
                    0.0f,
                    0
                ));

                return;
            }

            //Ignoring accelerometer updates while GPS not fixed
            if (LocationSensorStatus != LocationSensorStatus.Working) {
                return;
            }

            //Ignoring while we have invalid values from GPS (if fixed this shouldn't happen anyway)
            if (!(_gpsLastLatitude.IsValid() && _gpsLastLongitude.IsValid() &&
                  _gpsLastBearing.IsValid())) {
                return;
            }

            //TODO: check for acceleration sampling frequency here

            //TODO: computation is now sync-locked to the accelerometer instead of the intended
            //      100 Hz frequency. This should be changed.

            //NOTE: cannot use the sensor timestamp data, as this is HW dependent on Android
            //      see https://code.google.com/p/android/issues/detail?id=56561
            var timestamp = DateTime.UtcNow;

            //Check for unfixed GPS timeout
            if (timestamp - _gpsLastUpdate > GpsUnfixedTimeout) {
                LocationSensorStatus = LocationSensorStatus.Fixing;
                return;
            }

            //Check whether GPS has exceeded the stationarity timeout
            if (!Settings.SuspensionDisabled && (timestamp - _lastTimeMoved > GpsDistanceErrorTimeout)) {
                Debug("GPS stationary for {0}", timestamp - _lastTimeMoved);
                OnLocationSensorError(LocationErrorType.RemainedStationary);
                return;
            }

#if !DEBUG
            //Ignoring while the user is moving too slowly
            //TODO: skip n frames (or timeout) when speed gets acceptable
            if (_isMovingSlowly) {
                _engine.Reset();
                return;
            }
#endif

            if(timestamp - _gpsLastUpdate > TimeSpan.FromSeconds(1.5)) {
                // GPS data too old, skip for now
                return;
            }

            FeedEngine(new DataEntry(
                timestamp,
                accX, accY, accZ,
                _gpsLastLatitude, _gpsLastLongitude,
                _gpsLastSpeed,
                _gpsLastBearing,
                _gpsLastAccuracy
            ));
        }

        /// <summary>
        /// Feeds data to the engine, with safety checks.
        /// </summary>
        private void FeedEngine(DataEntry data) {
            try {
                _engine.Register(data);
            }
            catch (EngineComputationException ex) {
                Error(ex, "Internal engine computation error");
                _engine.Reset();

                OnInternalEngineErrorReported(ex);
            }
            catch (ArgumentException ex) {
                Debug("Engine data error ({0})", ex.Message);
                _engine.Reset();
            }
            catch (Exception ex) {
                Error(ex, "Unforeseen internal engine error");
                _engine.Reset();
            }
        }

#endregion Common sensor data handling

        private double _accelerometerScaleFactor = 1.0;

        public void StartSensing() {
            if (_isActive) {
                Debug("Sensors already running");
                return;
            }

            ResetGpsLocation();

            //GPS times are set to now as a starting point
            _gpsLastUpdate = DateTime.UtcNow;
            _lastTimeFastEnough = DateTime.MinValue;
            _lastTimeMoved = DateTime.MinValue;

            _accelerometerScaleFactor = Settings.CalibrationScaleFactor;
            Debug("Sensor pack using scale factor of {0:P2}", _accelerometerScaleFactor);

            try {
                StartSensingCore();
            }
            catch(Exception ex) {
                Error(ex, "Failed started sensing");
                _isActive = false;
                return;
            }
            Debug("Sensor pack started sensing");
            _isActive = true;
        }

        protected abstract void StartSensingCore();

        public void StopSensing() {
            if (!_isActive) {
                Debug("Sensors already stopped");
                return;
            }

            StopSensingCore();
            Debug("Sensor pack stopped sensing");
            _isActive = false;
        }

        protected abstract void StopSensingCore();

        /// <summary>
        /// Resets the GPS location to an untracked position and resets computation.
        /// </summary>
        private void ResetGpsLocation() {
            _gpsLastLatitude = double.NaN;
            _gpsLastLongitude = double.NaN;
            _gpsLastSpeed = float.NaN;
            _gpsLastBearing = 0f;
            _gpsLastAccuracy = 0;

            _engine.Reset();
        }

    }

}
