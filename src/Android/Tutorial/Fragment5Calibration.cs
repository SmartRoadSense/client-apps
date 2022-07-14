using System;
using Android.OS;
using Android.Views;
using Android.Widget;

using SmartRoadSense.Shared;
using SmartRoadSense.Shared.Calibration;

namespace SmartRoadSense.Android.Tutorial {

    public class Fragment5Calibration : global::AndroidX.Fragment.App.Fragment {

        private readonly Calibrator _calibrator;

        public Fragment5Calibration() {
            _calibrator = Calibrator.Create();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            var view = inflater.Inflate(Resource.Layout.fragment_tutorial_5_calibration, container, false);

            view.FindViewById<Button>(Resource.Id.button_calibrate).Click += HandleCalibrateClick;

            return view;
        }

        public override void OnResume() {
            base.OnResume();

            SetUiState(View, Settings.CalibrationDone);
        }

        private void SetUiState(View root, bool completed) {
            if (root == null)
                return;

            root.FindViewById<TextView>(Resource.Id.text_instruction).Visibility = completed.TrueToGone();
            root.FindViewById<TextView>(Resource.Id.text_confirmation).Visibility = completed.FalseToGone();

            var button = root.FindViewById<Button>(Resource.Id.button_calibrate);
            button.Enabled = !completed;
            button.Visibility = completed.TrueToGone();
        }

        private async void HandleCalibrateClick(object sender, EventArgs e) {
            if (SmartRoadSenseApplication.CalibrationUi.IsCalibrating)
                return;

            var result = await SmartRoadSenseApplication.CalibrationUi.Calibrate(Activity);

            SetUiState(View, result == CalibrationResult.Completed);
        }

    }

}
