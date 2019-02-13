#if __IOS__

using System;
using CoreMotion;
using SmartRoadSense.Shared.Data;
using Foundation;

namespace SmartRoadSense.Shared.Calibration {

    public class CalibratorIOS : Calibrator {

        private readonly CMMotionManager _sensorManager;

        public CalibratorIOS() {
            _sensorManager = new CMMotionManager ();
        }

#region Implemented abstract members of Calibrator

        protected override void StartSensor() {
            _sensorManager.AccelerometerUpdateInterval = 1.0 / SensorPack.AccelerometerDesiredHz;
            _sensorManager.StartAccelerometerUpdates (NSOperationQueue.CurrentQueue, (data, error) => {
                if (error == null) {
                    double accX = data.Acceleration.X;
                    double accY = data.Acceleration.Y;
                    double accZ = data.Acceleration.Z;

                    ReportAcceleration(accX, accY, accZ);
                }

                // TODO: catch error
            });
        }

        protected override void StopSensor() {
            _sensorManager.StopAccelerometerUpdates ();
        }

#endregion Implemented abstract members of Calibrator

    }

}

#endif
