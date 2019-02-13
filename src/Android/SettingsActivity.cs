using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using SmartRoadSense.Shared;

namespace SmartRoadSense.Android {
    
    [Activity(
        Label = "@string/Vernacular_P0_title_settings",
        ParentActivity = typeof(MainActivity)
    )]
    public class SettingsActivity : AppCompatActivity {

        private class VehicleTypeAdapter : BaseAdapter {

            public VehicleTypeAdapter()
                : base() {
            }

            public override int Count {
                get {
                    return 3;
                }
            }

            public override Java.Lang.Object GetItem(int position) {
                return null;
            }

            public override long GetItemId(int position) {
                return position;
            }

            public static VehicleType GetVehicleType(int position) {
                return (VehicleType)(position + 1);
            }

            public static int GetPosition(VehicleType type) {
                return ((int)type - 1);
            }

            public override View GetView(int position, View convertView, ViewGroup parent) {
                if (convertView == null) {
                    var inflater = (LayoutInflater)App.Context.GetSystemService(Context.LayoutInflaterService);
                    convertView = inflater.Inflate(Resource.Layout.custom_spinner_item, parent, false);
                }

                var vehicle = GetVehicleType(position);

                convertView.FindViewById<ImageView>(Resource.Id.image).SetImageResource(vehicle.GetIconId());
                convertView.FindViewById<TextView>(global::Android.Resource.Id.Text1).Text = App.Context.GetString(vehicle.GetStringId());

                return convertView;
            }

            public override View GetDropDownView(int position, View convertView, ViewGroup parent) {
                if (convertView == null) {
                    var inflater = (LayoutInflater)App.Context.GetSystemService(Context.LayoutInflaterService);
                    convertView = inflater.Inflate(global::Android.Resource.Layout.SimpleSpinnerDropDownItem, parent, false);
                }

                convertView.FindViewById<TextView>(global::Android.Resource.Id.Text1).Text = App.Context.GetString(GetVehicleType(position).GetStringId());

                return convertView;
            }

        }

        private class AnchorageTypeAdapter : BaseAdapter {

            public AnchorageTypeAdapter()
                : base() {
            }

            public override int Count {
                get {
                    return 3;
                }
            }

            public override Java.Lang.Object GetItem(int position) {
                return null;
            }

            public override long GetItemId(int position) {
                return position;
            }

            public static AnchorageType GetAnchorageType(int position) {
                return (AnchorageType)(position + 1);
            }

            public static int GetPosition(AnchorageType type) {
                return ((int)type - 1);
            }

            public override View GetView(int position, View convertView, ViewGroup parent) {
                if (convertView == null) {
                    var inflater = (LayoutInflater)App.Context.GetSystemService(Context.LayoutInflaterService);
                    convertView = inflater.Inflate(Resource.Layout.custom_spinner_item, parent, false);
                }

                var anchorage = GetAnchorageType(position);

                convertView.FindViewById<ImageView>(Resource.Id.image).SetImageResource(anchorage.GetIconId());
                convertView.FindViewById<TextView>(global::Android.Resource.Id.Text1).Text = App.Context.GetString(anchorage.GetStringId());

                return convertView;
            }

            public override View GetDropDownView(int position, View convertView, ViewGroup parent) {
                if (convertView == null) {
                    var inflater = (LayoutInflater)App.Context.GetSystemService(Context.LayoutInflaterService);
                    convertView = inflater.Inflate(global::Android.Resource.Layout.SimpleSpinnerDropDownItem, parent, false);
                }

                convertView.FindViewById<TextView>(global::Android.Resource.Id.Text1).Text = App.Context.GetString(GetAnchorageType(position).GetStringId());

                return convertView;
            }

        }

        private CheckBox _checkStartAtBoot,
                         _checkPreferUnmetered,
                         _checkDisableSuspension,
                         _checkOfflineMode;
        private Spinner _spinnerVehicle, _spinnerAnchorage;

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_settings);

            //Toolbar support
            var toolbar = this.FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if(toolbar != null) {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            if(Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                FindViewById<View>(Resource.Id.toolbar_shadow).Visibility = ViewStates.Gone;
            }

            _checkStartAtBoot = FindViewById<CheckBox>(Resource.Id.checkbox_start_at_boot);
            _checkStartAtBoot.CheckedChange += (sender, e) => {
                Settings.StartAtBoot = e.IsChecked;
            };

            _checkPreferUnmetered = FindViewById<CheckBox>(Resource.Id.checkbox_prefer_unmetered);
            _checkPreferUnmetered.CheckedChange += (sender, e) => {
                Settings.PreferUnmeteredConnection = e.IsChecked;
            };

            _checkDisableSuspension = FindViewById<CheckBox>(Resource.Id.checkbox_disable_suspension);
            _checkDisableSuspension.CheckedChange += (sender, e) => {
                Settings.SuspensionDisabled = e.IsChecked;
            };

            _checkOfflineMode = FindViewById<CheckBox>(Resource.Id.checkbox_offline_mode);
            _checkOfflineMode.CheckedChange += (sender, e) => {
                Settings.OfflineMode = e.IsChecked;
            };

            _spinnerVehicle = FindViewById<Spinner>(Resource.Id.spinner_vehicle);
            _spinnerVehicle.Adapter = new VehicleTypeAdapter();
            _spinnerVehicle.ItemSelected += (sender, e) => {
                Settings.LastVehicleType = VehicleTypeAdapter.GetVehicleType(e.Position);
            };

            _spinnerAnchorage = FindViewById<Spinner>(Resource.Id.spinner_anchorage);
            _spinnerAnchorage.Adapter = new AnchorageTypeAdapter();
            _spinnerAnchorage.ItemSelected += (sender, e) => {
                Settings.LastAnchorageType = AnchorageTypeAdapter.GetAnchorageType(e.Position);
            };

            var buttonTutorial = FindViewById<Button>(Resource.Id.button_redo);
            buttonTutorial.Click += HandleRedoTutorialClick;

            var buttonRecalibrate = FindViewById<Button>(Resource.Id.button_recalibrate);
            buttonRecalibrate.Click += HandleRecalibrateClick;
        }

        private void HandleRedoTutorialClick(object sender, EventArgs e) {
            this.StartActivity(typeof(TutorialActivity));
        }

        private async void HandleRecalibrateClick(object sender, EventArgs e) {
            if(SmartRoadSenseApplication.CalibrationUi.IsCalibrating)
                return;

            await SmartRoadSenseApplication.CalibrationUi.Calibrate(this);

            RefreshCalibrationInfo();
        }

        protected override void OnResume() {
            base.OnResume();

            //Reload settings
            _checkStartAtBoot.Checked = Settings.StartAtBoot;
            _checkPreferUnmetered.Checked = Settings.PreferUnmeteredConnection;
            _checkDisableSuspension.Checked = Settings.SuspensionDisabled;
            _checkOfflineMode.Checked = Settings.OfflineMode;
            _spinnerVehicle.SetSelection(VehicleTypeAdapter.GetPosition(Settings.LastVehicleType));
            _spinnerAnchorage.SetSelection(AnchorageTypeAdapter.GetPosition(Settings.LastAnchorageType));

            RefreshCalibrationInfo();
        }

        protected override void OnDestroy() {
            base.OnDestroy();

            //Re-configure sync since user may have changed sync preferences
            DataSyncReceiver.AdaptiveConfigureSync(this).Forget();
        }

        private void RefreshCalibrationInfo() {
            var done = Settings.CalibrationDone;

            FindViewById<TextView>(Resource.Id.text_calibration_status).Text = (done) ?
                GetString(Resource.String.Vernacular_P0_calibration_done_info) :
                GetString(Resource.String.Vernacular_P0_calibration_not_done);

            var textFactor = FindViewById<TextView>(Resource.Id.text_calibration_scale_factor);
            textFactor.Text = string.Format(GetString(Resource.String.Vernacular_P0_calibration_scale), Settings.CalibrationScaleFactor);
            var textDetails = FindViewById<TextView>(Resource.Id.text_calibration_details);
            textDetails.Text = string.Format(GetString(Resource.String.Vernacular_P0_calibration_details),
                Settings.CalibrationOriginalMagnitudeMean, Settings.CalibrationOriginalMagnitudeStdDev);
            textFactor.Visibility = textDetails.Visibility = done.FalseToGone();
        }

    }

}

