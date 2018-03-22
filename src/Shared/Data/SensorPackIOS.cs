#if __IOS__

using System;
using System.Collections.Generic;
using CoreLocation;
using CoreMotion;
using Foundation;
using UIKit;

using SmartRoadSense.Core;

namespace SmartRoadSense.Shared.Data
{

	public class SensorPackIOS : SensorPack
	{

		public SensorPackIOS (Engine engine)
			: base (engine)
		{
			_locationManager = new CLLocationManager ();
			_locationManager.DesiredAccuracy = DesiredAccuracy;
			_locationManager.PausesLocationUpdatesAutomatically = false; 

			_locationManager.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) => {

				if(_locationManager.Location.Coordinate.IsValid ()){
					ReportNewLocation (
						_locationManager.Location.Coordinate.Latitude,
						_locationManager.Location.Coordinate.Longitude,
						(float)_locationManager.Location.Speed,
						(float)_locationManager.Location.Course,
						(int)_locationManager.Location.HorizontalAccuracy
					);
				} else 
					Log.Debug ("LocationManager Coordinates are not valid.");
			};

			_locationManager.LocationUpdatesPaused += (object sender, EventArgs e) => {
				LocationSensorStatus = LocationSensorStatus.Disabled;
			};

			_locationManager.LocationUpdatesResumed += (object sender, EventArgs e) => {
				LocationSensorStatus = LocationSensorStatus.Working;
			};

			_locationManager.AuthorizationChanged += (object sender, CLAuthorizationChangedEventArgs e) => {
				if (e.Status == CLAuthorizationStatus.Denied)
					LocationSensorStatus = LocationSensorStatus.Disabled;
				else if (e.Status == CLAuthorizationStatus.NotDetermined)
					LocationSensorStatus = LocationSensorStatus.Disabled;
				else if (e.Status == CLAuthorizationStatus.Restricted)
					LocationSensorStatus = LocationSensorStatus.Disabled;
			};
				
			_sensorManager = new CMMotionManager ();
		}

		private readonly CLLocationManager _locationManager;
		private readonly CMMotionManager _sensorManager;
		private const int DesiredAccuracy = 20;

		protected override void StartSensingCore ()
		{
			// iOS 8 has additional permissions requirements
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				_locationManager.RequestAlwaysAuthorization (); // works in background
				//locMgr.RequestWhenInUseAuthorization (); // only in foreground
			}

			if (UIDevice.CurrentDevice.CheckSystemVersion (9, 0)) {
				_locationManager.AllowsBackgroundLocationUpdates = true;
			}

			_locationManager.StartUpdatingLocation ();

            _sensorManager.AccelerometerUpdateInterval = 1.0 / SensorPack.AccelerometerDesiredHz;
			_sensorManager.StartAccelerometerUpdates (NSOperationQueue.CurrentQueue, (data, error) => {
				if (error == null) {
					ReportNewAcceleration (data.Acceleration.X, data.Acceleration.Y, data.Acceleration.Z);
				}
				else {
					//TODO: catch error
				}
			});
		}

		protected override void StopSensingCore ()
		{
			_locationManager.StopUpdatingLocation ();
			_sensorManager.StopAccelerometerUpdates ();
		}

	}

}

#endif
