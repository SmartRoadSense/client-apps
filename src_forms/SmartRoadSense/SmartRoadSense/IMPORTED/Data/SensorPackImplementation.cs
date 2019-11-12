using System;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using SmartRoadSense.Core;
using Xamarin.Essentials;

namespace SmartRoadSense
{
    public class SensorPackImplementation : SensorPack
    {
        private const int DesiredAccuracy = 20;

        public SensorPackImplementation(Engine engine) : base(engine)
        {
            if(!CrossGeolocator.IsSupported)
            {
                Log.Debug("Geolocator not supported, location not available.");
                return;
            }

            if (!CrossGeolocator.Current.IsListening)
            {
                Log.Debug("Geolocator not listening, location not available.");
                return;
            }

            CrossGeolocator.Current.DesiredAccuracy = DesiredAccuracy;

            // TODO: change LocationSensorStatus on location status changes?

            /*
            _locationManager.LocationUpdatesPaused += (object sender, EventArgs e) => {
                LocationSensorStatus = LocationSensorStatus.Disabled;
            };

            _locationManager.LocationUpdatesResumed += (object sender, EventArgs e) => {
                LocationSensorStatus = LocationSensorStatus.Working;
            };

            _locationManager.AuthorizationChanged += (object sender, CLAuthorizationChangedEventArgs e) =>
            {
                if (e.Status == CLAuthorizationStatus.Denied)
                    LocationSensorStatus = LocationSensorStatus.Disabled;
                else if (e.Status == CLAuthorizationStatus.NotDetermined)
                    LocationSensorStatus = LocationSensorStatus.Disabled;
                else if (e.Status == CLAuthorizationStatus.Restricted)
                    LocationSensorStatus = LocationSensorStatus.Disabled;
            };
            */
        }

        private void Current_PositionChanged(object sender, PositionEventArgs e)
        {
            if(e == null)
            {
                Log.Debug("PositionEvent is not available.");
                return;
            }

            if (e.Position != null)
            {
                ReportNewLocation(
                    e.Position.Latitude,
                    e.Position.Longitude,
                    (float)e.Position.Speed,
                    (float)e.Position.Heading,
                    (int)e.Position.Accuracy
                );
            }
            else
                Log.Debug("LocationManager Coordinates are not available.");
        }

        async Task InitBackgroundLocationService()
        {
            await CrossGeolocator.Current.StartListeningAsync(
                TimeSpan.FromSeconds(1),
                1,
                true,
                new ListenerSettings
                {
                    ActivityType = ActivityType.AutomotiveNavigation,
                    AllowBackgroundUpdates = true,
                    DeferLocationUpdates = true,
                    DeferralDistanceMeters = 1,
                    DeferralTime = TimeSpan.FromSeconds(1),
                    ListenForSignificantChanges = true,
                    PauseLocationUpdatesAutomatically = false
                }
            );
        }

        protected override void StartSensingCore()
        {
            // TODO: set accelerometer hz?

            // Accelerometer
            Accelerometer.Start(SensorSpeed.Fastest);
            Accelerometer.ReadingChanged += ReadingChanged_EventArgs;

            // Location
            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
            _ = InitBackgroundLocationService();
        }

        protected override void StopSensingCore()
        {
            // Accelerometer
            Accelerometer.Stop();
            Accelerometer.ReadingChanged -= ReadingChanged_EventArgs;

            // Location
            CrossGeolocator.Current.StopListeningAsync();
            CrossGeolocator.Current.PositionChanged -= Current_PositionChanged;
        }

        void ReadingChanged_EventArgs(object s, AccelerometerChangedEventArgs e)
        {
            if (Accelerometer.IsMonitoring)
                ReportNewAcceleration(e.Reading.Acceleration.X, e.Reading.Acceleration.Y, e.Reading.Acceleration.Z);
        }
    }
}
