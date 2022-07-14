using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Views;
using Android.Widget;

using SmartRoadSense.Shared;
using SmartRoadSense.Shared.Data;

using Plugin.CurrentActivity;
using AndroidX.DrawerLayout.Widget;
using AndroidX.Core.Content;
using SmartRoadSense.Resources;

namespace SmartRoadSense.Android {

    [Activity(
        #if BETA
        Label = "@string/Vernacular_P0_title_main_beta",
        #elif DEBUG
        Label = "@string/Vernacular_P0_title_main_debug",
        #else
        Label = "@string/Vernacular_P0_title_main",
        #endif
        MainLauncher = true,
        LaunchMode = global::Android.Content.PM.LaunchMode.SingleTop,
        ConfigurationChanges = global::Android.Content.PM.ConfigChanges.KeyboardHidden
    )]
    public class MainActivity : AppCompatActivity {
        
        public const string IntentStartRecording = "it.uniurb.smartroadsense.ui.start_recording";

        private DrawerLayout _drawerLayout;

        private CustomActionBarDrawerToggle _drawerToggle;

        private ImageView _buttonRecord;

        private ImageButton _buttonStop;

        private TextView _textCurrPpe;

        private View _containerSetup;

        private ImageView _buttonSetupVehicle, _buttonSetupAnchorage;

        private MessageSnackbarDisplayer _bottomInfoDisplayer;

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            CrossCurrentActivity.Current.Init(Application);

            //Show tutorial if needed
            if(!Settings.DidShowTutorial) {
                StartActivity(typeof(TutorialActivity));
            }

            SetContentView(Resource.Layout.activity_main);

            //Toolbar support
            var toolbar = FindViewById<global::AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            if(toolbar != null) {
                SetSupportActionBar(toolbar);
            }
            if(Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                FindViewById<View>(Resource.Id.toolbar_shadow).Visibility = ViewStates.Gone;
            }

            //Setup UI elements
            _buttonRecord = FindViewById<ImageView>(Resource.Id.button_record);
            _buttonRecord.Click += HandleStartButtonClick;

            _buttonStop = FindViewById<ImageButton>(Resource.Id.button_stop);
            _buttonStop.Click += HandleStopButtonClick;

            _textCurrPpe = FindViewById<TextView>(Resource.Id.text_curr_ppe);
            _textCurrPpe.Visibility = ViewStates.Invisible;

            _containerSetup = FindViewById<View>(Resource.Id.container_setup);
            _containerSetup.Visibility = ViewStates.Invisible;

            _buttonSetupVehicle = FindViewById<ImageView>(Resource.Id.button_setup_vehicle);
            _buttonSetupVehicle.Click += HandleSetupClick;
            _buttonSetupAnchorage = FindViewById<ImageView>(Resource.Id.button_setup_anchorage);
            _buttonSetupAnchorage.Click += HandleSetupClick;

            this.InitNavigationDrawer(toolbar);

            _bottomInfoDisplayer = new MessageSnackbarDisplayer(this, FindViewById<View>(Resource.Id.snackbar_container), _buttonStop);

            BindToService();

            HandleIntent(this.Intent);
        }

        protected override void OnPostCreate(Bundle savedInstanceState) {
            base.OnPostCreate(savedInstanceState);
            _drawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(global::Android.Content.Res.Configuration newConfig) {
            base.OnConfigurationChanged(newConfig);
            _drawerToggle.OnConfigurationChanged(newConfig);
        }

        public override void OnBackPressed() {
            if(_drawerLayout.IsDrawerOpen((int)(GravityFlags.Left | GravityFlags.Start))) {
                _drawerLayout.CloseDrawers();
                return;
            }
            base.OnBackPressed();
        }

        public override bool OnOptionsItemSelected(IMenuItem item) {
            if(_drawerToggle.OnOptionsItemSelected(item)) {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnNewIntent(Intent intent) {
            base.OnNewIntent(intent);
            HandleIntent(intent);
        }

        protected override void OnResume() {
            base.OnResume();
            RefreshUi();
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            UnbindFromService();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults) {
            if(requestCode == PermissionRequestLocation) {
                var index = Array.IndexOf<string>(permissions, Manifest.Permission.AccessFineLocation);
                if(index >= 0) {
                    if(grantResults[index] == Permission.Granted) {
                        Log.Debug("Permission {0} granted", Manifest.Permission.AccessFineLocation);
                        HandleStartButtonClick(this, EventArgs.Empty);
                        return;
                    }
                }

                Log.Debug("Permission {0} not granted", Manifest.Permission.AccessFineLocation);
                Toast.MakeText(ApplicationContext, Resource.String.Vernacular_P0_error_permission_location_denied, ToastLength.Long).Show();
            }
        }

        private const int PermissionRequestLocation = 123;

        private void HandleStartButtonClick(object sender, EventArgs e) {
            if(SensingService.ViewModel == null || SensingService.ViewModel.IsRecording) {
                // Already recording (or some view model binding error), ignore
                return;
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M) {
                if (ContextCompat.CheckSelfPermission(App.Context, Manifest.Permission.AccessFineLocation) != global::Android.Content.PM.Permission.Granted) {
                    RequestPermissions(new string[] {
                        Manifest.Permission.AccessFineLocation
                    }, PermissionRequestLocation);

                    return;
                }
            }

            var d = new DialogRideSharing();
            d.Selected += (dsender, de) => {
                Log.Debug("Last number of people: from {0} to {1}", Settings.LastNumberOfPeople, de.Value);
                Settings.LastNumberOfPeople = de.Value;

                if (de.Value == 1 && !Settings.DoNotAskForCarpooling) {
                    // Only one passenger, show carpooling dialog
                    var dialogBlaBlaCar = new DialogBlaBlaCar();
                    dialogBlaBlaCar.Closed += (dbsender, dbe) => {
                        StartRecording();
                    };
                    dialogBlaBlaCar.Show(FragmentManager, "blablacar_dialog");
                }
                else {
                    StartRecording();
                }
            };
            d.Show(FragmentManager, "ride_sharing_dialog");
        }

        private void HandleStopButtonClick(object sender, EventArgs e) {
            StopRecording();
        }

        #region Service binding and view model event handling

        private void BindToService() {
            SensingService.ViewModel.MeasurementsUpdated += HandleMeasurementsUpdated;
            SensingService.ViewModel.RecordingStatusUpdated += HandleRecordingStatusUpdated;
            SensingService.ViewModel.SensorStatusUpdated += HandleSensorStatusUpdated;
            SensingService.ViewModel.RecordingSuspended += HandleRecordingSuspended;
            SensingService.ViewModel.InternalEngineErrorReported += HandleInternalEngineError;
            SensingService.ViewModel.SyncErrorReported += HandleSyncError;
        }

        private void UnbindFromService() {
            SensingService.ViewModel.MeasurementsUpdated -= HandleMeasurementsUpdated;
            SensingService.ViewModel.RecordingStatusUpdated -= HandleRecordingStatusUpdated;
            SensingService.ViewModel.SensorStatusUpdated -= HandleSensorStatusUpdated;
            SensingService.ViewModel.RecordingSuspended -= HandleRecordingSuspended;
            SensingService.ViewModel.InternalEngineErrorReported -= HandleInternalEngineError;
            SensingService.ViewModel.SyncErrorReported -= HandleSyncError;
        }

        private void HandleSetupClick(object sender, EventArgs e) {
            Intent i = new Intent(this, typeof(SettingsActivity));
            StartActivity(i);
        }

        private readonly ArgbEvaluator _argbEvaluator = new ArgbEvaluator();

        void HandleMeasurementsUpdated(object sender, EventArgs e) {
            Log.Debug("New engine measurement: {0:F2}, GPS {1}", SensingService.ViewModel.CurrentPpe, SensingService.ViewModel.LocationSensorStatus);
            if(SensingService.ViewModel != null) {
                UpdateMeasurementsDisplay();
            }
        }

        private const int FadeInDelay = 2000;

        private const int FadeOutDelay = 500;

        void HandleRecordingStatusUpdated(object sender, EventArgs e) {
            if(SensingService.ViewModel != null) {
                UpdateRecordButtonUi();
                UpdateSetupUi();
                if(SensingService.ViewModel.IsRecording) {
                    //Remove internal error message when switching to recording status
                    _bottomInfoDisplayer.Hide(InformationMessage.InternalEngineError);
                    _bottomInfoDisplayer.ShowButton(false);
                    _textCurrPpe.FadeIn(FadeInDelay);
                    _containerSetup.FadeIn(FadeInDelay);
                }
                else {
                    _bottomInfoDisplayer.HideButton(false);
                    _textCurrPpe.FadeOut(FadeOutDelay);
                    _containerSetup.FadeOut(FadeOutDelay);
                }
            }
        }

        void HandleSensorStatusUpdated(object sender, EventArgs e) {
            if(SensingService.ViewModel != null) {
                UpdateMeasurementsDisplay();
                UpdateRecordButtonUi();
                UpdateBottomBarUi();
            }
        }

        private void HandleRecordingSuspended(object sender, LocationErrorEventArgs e) {
            switch(e.Error) {
            case LocationErrorType.RemainedStationary:
                _bottomInfoDisplayer.Show(InformationMessage.GpsSuspendedStationary, MessageSnackbarDisplayer.LongDuration);
                break;
            case LocationErrorType.SpeedTooLow:
                _bottomInfoDisplayer.Show(InformationMessage.GpsSuspendedSpeed, MessageSnackbarDisplayer.LongDuration);
                break;
            }
        }

        private void HandleInternalEngineError(object sender, EventArgs e) {
            _bottomInfoDisplayer.Show(InformationMessage.InternalEngineError);
        }

        private void HandleSyncError(object sender, SyncErrorEventArgs e) {
            RunOnUiThread(() => {
                _bottomInfoDisplayer.Show(InformationMessage.UploadFailure, MessageSnackbarDisplayer.LongDuration);
            });
        }

        private void UpdateRecordButtonUi() {
            if(SensingService.ViewModel != null) {
                if(SensingService.ViewModel.IsRecording) {
                    if(SensingService.ViewModel.LocationSensorStatus.IsActive()) {
                        _buttonRecord.SetImageResource(Resource.Drawable.button_car_active_selector);
                    }
                    else {
                        _buttonRecord.SetImageResource(Resource.Drawable.button_car_unknown_selector);
                    }
                }
                else {
                    _buttonRecord.SetImageResource(Resource.Drawable.button_car_normal_selector);
                }
            }
        }

        private void UpdateSetupUi() {
            _buttonSetupVehicle.SetImageResource(Settings.LastVehicleType.GetIconId());
            _buttonSetupAnchorage.SetImageResource(Settings.LastAnchorageType.GetIconId());
        }

        private const string MeasurementFormat = "{0:F1}";

        /// <summary>
        /// Instantaneously updates the measurements display.
        /// Assumes the ViewModel is available.
        /// </summary>
        private void UpdateMeasurementsDisplay() {
            if(SensingService.ViewModel.IsReporting) {
                _textCurrPpe.Text = string.Format(MeasurementFormat, SensingService.ViewModel.CurrentPpe);
                _textCurrPpe.SetTextColor(PpeColorMapper.Map(SensingService.ViewModel.CurrentPpe).Color);
            }
            else {
                _textCurrPpe.Text = GetString(Resource.String.Vernacular_P0_unknown_ppe_value);
                _textCurrPpe.SetTextColor(Resources.GetColor(Resource.Color.quality_unknown));
            }
        }

        /// <summary>
        /// Updates the bottom bar UI based on the current status.
        /// Assumes the ViewModel is available.
        /// </summary>
        private void UpdateBottomBarUi() {
            //GPS status information bar
            if(SensingService.ViewModel.IsRecording) {
                var sensorStatus = SensingService.ViewModel.LocationSensorStatus;
                _bottomInfoDisplayer.ShowIf(InformationMessage.GpsDisabled, sensorStatus == LocationSensorStatus.Disabled);
                _bottomInfoDisplayer.Hide(InformationMessage.GpsSuspendedStationary);
                _bottomInfoDisplayer.ShowIf(InformationMessage.GpsUnfixed, sensorStatus == LocationSensorStatus.Fixing);
                //TODO: out of country
            }
        }

        /// <summary>
        /// Forces a quick refresh of the whole UI.
        /// </summary>
        private void RefreshUi() {
            if(SensingService.ViewModel != null) {
                UpdateRecordButtonUi();
                UpdateSetupUi();
                if(SensingService.ViewModel.IsRecording) {
                    UpdateMeasurementsDisplay();
                    _bottomInfoDisplayer.ShowButton(true);
                    _textCurrPpe.Visibility = _containerSetup.Visibility = ViewStates.Visible;
                }
                else {
                    _bottomInfoDisplayer.HideButton(true);
                    _textCurrPpe.Visibility = _containerSetup.Visibility = ViewStates.Invisible;
                }
                UpdateBottomBarUi();
            }
        }

        #endregion

        /// <summary>
        /// Initializes the navigation drawer menu.
        /// </summary>
        private void InitNavigationDrawer(global::AndroidX.AppCompat.Widget.Toolbar toolbar) {
            _drawerLayout = this.FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _drawerToggle = new CustomActionBarDrawerToggle(this, _drawerLayout, toolbar, Resource.String.Vernacular_P0_action_drawer_open, Resource.String.Vernacular_P0_action_drawer_close);
            _drawerLayout.AddDrawerListener(_drawerToggle);
            FindViewById<Button>(Resource.Id.navigation_button_settings).Click += (sender, e) => {
                Intent i = new Intent(this, typeof(SettingsActivity));
                StartActivity(i);
                _drawerLayout.CloseDrawers();
            };
            FindViewById<Button>(Resource.Id.navigation_button_log).Click += (sender, e) => {
                Intent i = new Intent(this, typeof(LogActivity));
                StartActivity(i);
                _drawerLayout.CloseDrawers();
            };
            FindViewById<Button>(Resource.Id.navigation_button_queue).Click += (sender, e) => {
                Intent i = new Intent(this, typeof(QueueActivity));
                StartActivity(i);
                _drawerLayout.CloseDrawers();
            };
            FindViewById<Button>(Resource.Id.navigation_button_stats).Click += (sender, e) => {
                Intent i = new Intent(this, typeof(StatsActivity));
                StartActivity(i);
                _drawerLayout.CloseDrawers();
            };
            FindViewById<Button>(Resource.Id.navigation_button_about).Click += (sender, e) => {
                Intent i = new Intent(this, typeof(AboutActivity));
                StartActivity(i);
                _drawerLayout.CloseDrawers();
            };
            FindViewById<Button>(Resource.Id.navigation_button_game).Click += (sender, e) => {
                Intent i = new Intent(this, typeof(GameActivity));
                StartActivity(i);
                _drawerLayout.CloseDrawers();
            };

            _drawerLayout.DrawerStateChanged += (sender, e) => {
                if(e.NewState == DrawerLayout.StateDragging || e.NewState == DrawerLayout.StateSettling) {
                    // Drawer movement, stop recording
                    SensingService.Do(model => {
                        if(model.IsRecording) {
                            Toast.MakeText(ApplicationContext, UiStrings.MainNotificationSuspendUserAction, ToastLength.Short).Show();
                            model.StopRecordingCommand.Execute(null);
                        }
                    });
                }
            };
        }

        private void HandleIntent(Intent intent) {
            if(intent == null)
                return;
            var action = intent.Action;
            if(IntentStartRecording.Equals(action, StringComparison.InvariantCultureIgnoreCase)) {
                StartRecording();
            }
            else {
                Log.Debug("Ignoring intent {0}", intent.Action);
            }
        }

        private void StartRecording() {
            SensingService.Do(model => {
                model.StartRecordingCommand.Execute(null);
            });
        }

        private void StopRecording() {
            SensingService.Do(model => {
                model.StopRecordingCommand.Execute(null);
            });
        }

    }

}
