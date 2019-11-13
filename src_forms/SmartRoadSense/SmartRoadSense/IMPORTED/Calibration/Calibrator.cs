using System;
using System.Threading.Tasks;

using SmartRoadSense.Core;

namespace SmartRoadSense {

    /// <summary>
    /// Calibrator that detects issues with the accelerometer data.
    /// </summary>
#if __ANDROID__
    /// <remarks>
    /// This class derives from Java.Lang.Object because of Xamarin limitations on Android.
    /// </remarks>
    public abstract class Calibrator : Java.Lang.Object {
#else
    public abstract class Calibrator {
#endif

        /// <summary>
        /// The size of the window over which the mean acceleration is computed.
        /// </summary>
        private const int WindowSize = 300;

        /// <summary>
        /// Amount of samples that are dropped to allow the device to settle after interaction.
        /// </summary>
        private const int SampleDropCount = 50;

        /// <summary>
        /// Maximum tolerance in standard deviation to mean ratio.
        /// If this is higher the sensor values are too disturbed to be used.
        /// </summary>
        private const double Tolerance = 0.01;

        public const double ReferenceGravitationalAcceleration = 9.80665;

        //Prevent instantiation
        protected Calibrator() {
        }

        private TaskCompletionSource<CalibrationResult> _currentCalibration;

        public Task<CalibrationResult> Calibrate() {
            if (_currentCalibration != null) {
                throw new InvalidOperationException("Cannot calibrate while calibration already running");
            }

            Log.Debug("Starting calibration");

            _currentCalibration = new TaskCompletionSource<CalibrationResult>();

            _dropCounter = 100;
            _fill = 0;
            StartSensor();

            return _currentCalibration.Task;
        }

        protected abstract void StartSensor();

        protected abstract void StopSensor();

        protected abstract void ToggleSensor();

        private int _dropCounter = SampleDropCount;
        private int _fill;
        private readonly double[] _window = new double[WindowSize];

        protected void ReportAcceleration(double accX, double accY, double accZ) {
            if (_dropCounter-- > 0) {
                //Ignore value
                return;
            }

            if (_currentCalibration == null) {
                //Calibration complete
                return;
            }

            var magnitude = Math.Sqrt(accX * accX + accY * accY + accZ * accZ);
            //Log.Debug("{{{0:F3},{1:F3},{2:F3}}}, magnitude {3}", accX, accY, accZ, magnitude);

            _window[_fill++] = magnitude;

            if (_fill >= WindowSize) {
                var stats = _window.ComputeStats();
                var stdDevRatio = stats.StandardDeviation / stats.Average;
                Log.Debug("Calibration statistics: mean {0} std.dev {1} ({2:P2})", stats.Average, stats.Variance, stats.StandardDeviation);

                if (stdDevRatio < Tolerance) {
                    var scaleFactor = ReferenceGravitationalAcceleration / stats.Average;

                    //TODO: check scale factor somehow? (if too high or too low this might be fishy)

                    Log.Debug("Calibration succeeded: scale factor {0:F3}", scaleFactor);

                    SettingsManager.Instance.CalibrationDone = true;
                    SettingsManager.Instance.CalibrationScaleFactor = scaleFactor;
                    SettingsManager.Instance.CalibrationOriginalMagnitudeMean = stats.Average;
                    SettingsManager.Instance.CalibrationOriginalMagnitudeStdDev = stats.StandardDeviation;

                    TerminateCalibrationTask(CalibrationResult.Completed);
                }
                else {
                    Log.Debug("Calibration failed: std.dev/mean ratio ({0:P2}) higher than tolerance {1:P2}", stdDevRatio, Tolerance);

                    TerminateCalibrationTask(CalibrationResult.StandardDeviationTooHigh);
                }
            }
        }

        protected void TerminateCalibrationTask(CalibrationResult result) {
            StopSensor();

            if (_currentCalibration == null)
            {
                throw new InvalidOperationException();
            }

            _currentCalibration.SetResult(result);
            _currentCalibration = null;

            Log.Event("Calibration.terminate",
                "result", result.ToString()
            );
        }

        public static Calibrator Create() {
            return new CalibratorImplementation();
        }

    }

}
