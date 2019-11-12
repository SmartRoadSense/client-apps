using System;
using Xamarin.Essentials;

namespace SmartRoadSense
{
    public class CalibratorImplementation : Calibrator
    {
        #region Implemented abstract members of Calibrator

        protected override void StartSensor()
        {
            ToggleAccelerometer();
        }

        protected override void StopSensor()
        {
            ToggleAccelerometer();
        }

        protected override void ToggleSensor()
        {
            ToggleAccelerometer();
        }

        #endregion Implemented abstract members of Calibrator

        void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;

            double accX = data.Acceleration.X;
            double accY = data.Acceleration.Y;
            double accZ = data.Acceleration.Z;

            ReportAcceleration(accX, accY, accZ);
        }

        void ToggleAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                {
                    Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
                    Accelerometer.Stop();
                }
                else
                {
                    Accelerometer.Start(SensorSpeed.Fastest);
                    Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Log.Error(fnsEx, "CalibratorImplementation feature error");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "CalibratorImplementation generic error");
            }
        }
    }
}
