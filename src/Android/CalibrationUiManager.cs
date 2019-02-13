using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using SmartRoadSense.Shared.Calibration;

namespace SmartRoadSense.Android {

    public class CalibrationUiManager {

        private readonly Calibrator _calibrator;

        private int _failedAttempts = 0;

        public CalibrationUiManager() {
            _calibrator = Calibrator.Create();

            IsCalibrating = false;
        }

        public bool IsCalibrating { get; private set; }

        public async Task<CalibrationResult> Calibrate(Activity parentActivity) {
            if(IsCalibrating)
                throw new InvalidOperationException("Calibration operation already started");
            IsCalibrating = true;

            var dialog = new ProgressDialog(parentActivity);
            dialog.SetTitle(Resource.String.Vernacular_P0_dialog_calibration_title);
            dialog.SetMessage(parentActivity.GetString(Resource.String.Vernacular_P0_dialog_calibration_description));
            dialog.SetCancelable(false);
            dialog.SetIcon(Resource.Drawable.icon_accelerometer);
            dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            dialog.Indeterminate = true;
            dialog.Show();

            dialog.Window.AddFlags(WindowManagerFlags.KeepScreenOn);

            var result = await _calibrator.Calibrate();

            // Early termination if parent activity is not around anymore
            if(parentActivity.IsDestroyed || parentActivity.IsFinishing) {
                return CalibrationResult.Canceled;
            }
            dialog.Dismiss();

            if(result != CalibrationResult.Completed) {
                _failedAttempts++;

                Toast.MakeText(parentActivity, GetStringId(result), ToastLength.Long).Show();

                if(_failedAttempts >= 3) {
                    var errorDialog = new AlertDialog.Builder(parentActivity)
                        .SetTitle(Resource.String.Vernacular_P0_error_generic_reflective)
                        .SetMessage(Resource.String.Vernacular_P0_error_calibration_several_attempts)
                        .SetCancelable(true)
                        .SetPositiveButton(Resource.String.Vernacular_P0_dialog_ok, (IDialogInterfaceOnClickListener)null)
                        .SetIcon(Resource.Drawable.icon_accelerometer)
                        .Create();
                    errorDialog.Show();
                }
            }

            IsCalibrating = false;

            return result;
        }

        private int GetStringId(CalibrationResult result) {
            switch(result) {
                default:
                case CalibrationResult.Completed:
                    throw new ArgumentException();

                case CalibrationResult.StandardDeviationTooHigh:
                    return Resource.String.Vernacular_P0_error_calibration_high_deviation;
            }
        }

    }

}
