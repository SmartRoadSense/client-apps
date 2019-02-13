#if __ANDROID__

using System;

using Android.Hardware;
using Android.OS;
using Android.Content;

using SmartRoadSense.Shared.Data;

namespace SmartRoadSense.Shared.Calibration {
	
	public class CalibratorAndroid : Calibrator, ISensorEventListener {

		private readonly SensorManager _sensorManager;
		private readonly PowerManager.WakeLock _wakeLock;

		public const string WakeLockTag = "SmartRoadSense_Calibration_Wakelock";

		public CalibratorAndroid() {
			_sensorManager = (SensorManager)App.Context.GetSystemService(Context.SensorService);

			var pm = (PowerManager)App.Context.GetSystemService(Context.PowerService);
			_wakeLock = pm.NewWakeLock(WakeLockFlags.Partial, WakeLockTag);
		}

		#region implemented abstract members of Calibrator

		protected override void StartSensor() {
			_wakeLock.Acquire();

			_sensorManager.RegisterListener(this,
				_sensorManager.GetDefaultSensor(SensorType.Accelerometer),
				(SensorDelay)SensorPack.AccelerometerDesiredDelayMs);
		}

		protected override void StopSensor() {
			_sensorManager.UnregisterListener(this);

			if(_wakeLock.IsHeld) {
				_wakeLock.Release();
			}
		}

		#endregion

		#region ISensorEventListener implementation

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

			ReportAcceleration(accX, accY, accZ);
		}

		#endregion

	}

}

#endif
