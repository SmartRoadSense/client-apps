#if __ANDROID__

using System;

using Android;
using Android.Content;
using Android.Hardware;
using Android.Locations;
using Android.OS;
using AndroidX.Core.Content;
using SmartRoadSense.Core;

namespace SmartRoadSense.Shared.Data {

    public class SensorPackAndroid : SensorPack, GpsStatus.IListener, ILocationListener, ISensorEventListener {

        private readonly LocationManager _locationManager;
        private readonly SensorManager _sensorManager;
        private readonly PowerManager.WakeLock _wakeLock;

        public const string WakeLockTag = "SmartRoadSense_Sensing_Wakelock";

        public SensorPackAndroid(Engine engine)
            : base(engine) {

            _locationManager = (LocationManager)App.Context.GetSystemService(Context.LocationService);

            _sensorManager = (SensorManager)App.Context.GetSystemService(Context.SensorService);

            var pm = (PowerManager)App.Context.GetSystemService(Context.PowerService);
            _wakeLock = pm.NewWakeLock(WakeLockFlags.Partial, WakeLockTag);
        }

        protected override void StartSensingCore() {
            if(ContextCompat.CheckSelfPermission(App.Context, Manifest.Permission.AccessFineLocation) != global::Android.Content.PM.Permission.Granted) {
                throw new InvalidOperationException(string.Format("Cannot start location updates, missing {0} permission", Manifest.Permission.AccessFineLocation));
            }

            _wakeLock.Acquire();

            _locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 0, 0, this);

            _sensorManager.RegisterListener(this,
                _sensorManager.GetDefaultSensor(SensorType.Accelerometer),
                (SensorDelay)AccelerometerDesiredDelayMs);
        }

        protected override void StopSensingCore() {
            _locationManager.RemoveUpdates(this);

            _sensorManager.UnregisterListener(this);

            if(_wakeLock.IsHeld) {
                _wakeLock.Release();
            }
        }

        public void OnLocationChanged(Location location) {
            if(!LocationManager.GpsProvider.Equals(location.Provider)) {
                Log.Debug("Skipping non-GPS location data");
                return;
            }

            ReportNewLocation(
                location.Latitude,
                location.Longitude,
                location.Speed,
                location.Bearing,
                (int)location.Accuracy
            );
        }

        public void OnProviderEnabled(string provider) {
            if (LocationSensorStatus == LocationSensorStatus.Disabled) {
                LocationSensorStatus = LocationSensorStatus.Fixing;
            }
        }

        public void OnProviderDisabled(string provider) {
            LocationSensorStatus = LocationSensorStatus.Disabled;
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras) {
            //Irrelevant
        }

        public void OnGpsStatusChanged(GpsEvent e) {
            if (e == GpsEvent.FirstFix) {
                //We have fix for sure
                LocationSensorStatus = LocationSensorStatus.Working;
            }
            else if (e == GpsEvent.SatelliteStatus) {
                //Don't really care anymore
            }
        }

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy) {
            //Noop
        }

        public void OnSensorChanged(SensorEvent e) {
            if (e.Sensor.Type != SensorType.Accelerometer) {
                return;
            }

            double accX = e.Values[0];
            double accY = e.Values[1];
            double accZ = e.Values[2];

            ReportNewAcceleration(accX, accY, accZ);
        }

    }

}

#endif
