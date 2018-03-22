
using System;

using Foundation;
using UIKit;
using System.Runtime.Remoting.Channels;
using SmartRoadSense.Shared;
using CoreGraphics;
using System.Threading;
using BigTed;
using SmartRoadSense.Shared.Calibration;

namespace SmartRoadSense.iOS
{
	public partial class SettingsViewController : UIViewController
	{
		private String _stringPreferUnmeteredConnection = "StringPrefereUnmeteredConnection";
		private String _stringOfflineMode = "StringOfflineMode";
		private String _stringSuspention = "StringSuspention";

		private Calibrator _calibrator;
		private bool _calibrating = false;
		private int _failedAttempts = 0;

		public SettingsViewController (IntPtr handle) : base (handle)
		{
			this.Title = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_menu_settings", null);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var storyboard = UIStoryboard.FromName ("MainStoryboard", null);

			// Set texts
			lblOffline.Text  = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_settings_title_offline_mode", null);
			lblOfflineText.Text = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_settings_description_offline_mode", null);
			
            lblPosition.Text = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_settings_title_anchorage", null);
			lblPositionDescription.Text = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_settings_description_anchorage", null);
			
			lblRecording.Text = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_settings_section_setup", null).ToUpper ();
			
            lblUseWifi.Text = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_settings_title_prefer_unmetered", null);
			lblUseWifiText.Text = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_settings_description_prefer_unmetered", null);
			
            lblVehicle.Text = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_settings_title_vehicle", null);
			lblVehicleDescription.Text = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_settings_description_vehicle", null);
			
            lblCalibration.Text = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_settings_section_calibration", null).ToUpper ();
			btnCalibrate.SetTitle(NSBundle.MainBundle.LocalizedString("Vernacular_P0_calibration_forget", null), UIControlState.Normal);

			lblIntroduction.Text = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_settings_section_introduction", null).ToUpper ();
			btnIntro.SetTitle (NSBundle.MainBundle.LocalizedString ("Vernacular_P0_tutorial_redo", null), UIControlState.Normal);
			
            lblPreferences.Text = NSBundle.MainBundle.LocalizedString("Vernacular_P0_settings_section_preferences", null).ToUpper();

            lblDisableSuspention.Text = NSBundle.MainBundle.LocalizedString("Vernacular_P0_settings_title_disable_suspension", null);
            _lblDisableSuspentionText.Text = NSBundle.MainBundle.LocalizedString("Vernacular_P0_settings_description_disable_suspension", null);
			
            SetButtonTexts ();

			RefreshCalibrationInfo ();

			// Set text colors
			lblPreferences.TextColor = StyleSettings.ThemePrimaryColor ();
			lblRecording.TextColor = StyleSettings.ThemePrimaryColor ();
			lblCalibration.TextColor = StyleSettings.ThemePrimaryColor ();
			lblIntroduction.TextColor = StyleSettings.ThemePrimaryColor ();
			btnIntro.SetTitleColor (StyleSettings.TextOnDarkColor (), UIControlState.Normal);
			btnIntro.BackgroundColor = StyleSettings.ThemePrimaryColor ();
			btnIntro.Layer.CornerRadius = 5;
			btnCalibrate.SetTitleColor (StyleSettings.TextOnDarkColor (), UIControlState.Normal);
			btnCalibrate.BackgroundColor = StyleSettings.ThemePrimaryColor ();
			btnCalibrate.Layer.CornerRadius = 5;

			// Set button image size
			btnPosition.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnVehicle.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

			// Set switch status
			switchWifi.On = NSUserDefaults.StandardUserDefaults.BoolForKey (_stringPreferUnmeteredConnection);
			switchOffline.On = NSUserDefaults.StandardUserDefaults.BoolForKey (_stringOfflineMode);
            switchSuspention.On = NSUserDefaults.StandardUserDefaults.BoolForKey(_stringSuspention);

			// Init calibrator
			_calibrator = Calibrator.Create ();

			// Set wifi mode
			switchWifi.ValueChanged += (object sender, EventArgs e) => {
				if (switchWifi.On) {
					Settings.PreferUnmeteredConnection = true;
					NSUserDefaults.StandardUserDefaults.SetBool(true, _stringPreferUnmeteredConnection); 

				} else {
					Settings.PreferUnmeteredConnection = false;
					NSUserDefaults.StandardUserDefaults.SetBool(false, _stringPreferUnmeteredConnection); 
				}
			};

			// Set offline mode
			switchOffline.ValueChanged += (object sender, EventArgs e) => {
				if (switchOffline.On) {
					Settings.OfflineMode = true;
					NSUserDefaults.StandardUserDefaults.SetBool(true, _stringOfflineMode); 
				} else {
					Settings.OfflineMode = false;
					NSUserDefaults.StandardUserDefaults.SetBool(false, _stringOfflineMode); 
				}
			};

			// Set suspension status
            switchSuspention.ValueChanged += (object sender, EventArgs e) => {
                if (switchSuspention.On)
				{
					Settings.SuspensionDisabled = true;
                    NSUserDefaults.StandardUserDefaults.SetBool(true, _stringSuspention);
				}
				else
				{
					Settings.SuspensionDisabled = false;
					NSUserDefaults.StandardUserDefaults.SetBool(false, _stringSuspention);
				}
			};

			// change vehicle type
			btnVehicle.TouchUpInside += (object sender, EventArgs e) => {
				var SelectVehicleVC = storyboard.InstantiateViewController ("CustomPickerViewController") as CustomPickerViewController;
				SelectVehicleVC.pickerType = PreferencesSettings.VehicleTypePicker;
				SelectVehicleVC.SettingsVC = this;
				SelectVehicleVC.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
				SelectVehicleVC.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
				this.PresentViewController (SelectVehicleVC, true, null);
			};

			// change anchorage type
			btnPosition.TouchUpInside += (object sender, EventArgs e) => {
				var SelectAnchorageVC = storyboard.InstantiateViewController ("CustomPickerViewController") as CustomPickerViewController;
				SelectAnchorageVC.pickerType = PreferencesSettings.AnchorageTypePicker;
				SelectAnchorageVC.SettingsVC = this;
				SelectAnchorageVC.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
				SelectAnchorageVC.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
				this.PresentViewController (SelectAnchorageVC, true, null);
			};

			// Show intro
			btnIntro.TouchUpInside += (object sender, EventArgs e) => {
				IntroductionPageViewController introductionVC = storyboard.InstantiateViewController ("IntroductionPageViewController") as IntroductionPageViewController;
				this.NavigationController.PresentViewController (introductionVC, true, null);
			};

			// Re-calibrate
			btnCalibrate.TouchUpInside += (object sender, EventArgs e) => {
				if(_calibrating)
					return;
				else {
					_calibrating = true;
					ReCalibrate();
				}
			};

		}

		private async void ReCalibrate(){
			String title = NSBundle.MainBundle.LocalizedString("Vernacular_P0_dialog_calibration_title", null).PrepareForLabel ();
			String calibrating =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_dialog_calibration_description", null).PrepareForLabel ();

			// Show progress
			BTProgressHUD.Show (title + " " + calibrating);

			var result = await _calibrator.Calibrate ();
			CalibrationTerminated (result);
		}

		private void CalibrationTerminated(CalibrationResult result){

			// Hide progress
			BTProgressHUD.Dismiss ();
			if(result != CalibrationResult.Completed) {
				_failedAttempts++;

				var title = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_error_calibration_high_deviation", null).PrepareForLabel ();
				var ok = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_dialog_ok", null).PrepareForLabel ();

				//Create Alert
				var alertController = UIAlertController.Create(title, GetString(result), UIAlertControllerStyle.Alert);

				//Add Actions
				alertController.AddAction(UIAlertAction.Create(ok, UIAlertActionStyle.Default, null));

				//Present Alert
				PresentViewController(alertController, true, null);

				if(_failedAttempts >= 3) {

					var errorTitle = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_error_generic_reflective", null).PrepareForLabel ();
					var errorMessage = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_error_calibration_several_attempts", null).PrepareForLabel ();

					//Create Alert
					var errorAlertController = UIAlertController.Create(errorTitle, errorMessage, UIAlertControllerStyle.Alert);

					//Add Actions
					errorAlertController.AddAction(UIAlertAction.Create(ok, UIAlertActionStyle.Default, null));

					//Present Alert
					PresentViewController(errorAlertController, true, null);
				}
			}
			else {
				RefreshCalibrationInfo();
			}
			_calibrating = false;
		}

		private string GetString(CalibrationResult result) {
			switch(result) {
			default:
			case CalibrationResult.Completed:
				throw new ArgumentException();
			case CalibrationResult.StandardDeviationTooHigh:
				return NSBundle.MainBundle.LocalizedString ("Vernacular_P0_error_calibration_high_deviation", null).PrepareForLabel ();
			}
		}

		private void RefreshCalibrationInfo() {
			bool done = Settings.CalibrationDone;

			lblCalibrationInfo1.Text = done ? 
				NSBundle.MainBundle.LocalizedString("Vernacular_P0_calibration_done_info", null) : 
				NSBundle.MainBundle.LocalizedString("Vernacular_P0_calibration_not_done", null);
			lblCalibrationInfo2.Text = string.Format (
				NSBundle.MainBundle.LocalizedString ("Vernacular_P0_calibration_scale", null), 
				Settings.CalibrationScaleFactor
			);
			lblCalibrationInfo3.Text = string.Format (
				NSBundle.MainBundle.LocalizedString ("Vernacular_P0_calibration_details", null), 
				Settings.CalibrationOriginalMagnitudeMean, 
				Settings.CalibrationOriginalMagnitudeStdDev
			);

			lblCalibrationInfo2.Hidden = !done;
		}

		public void SetButtonTexts() {
			// Anchorage type
			if (Settings.LastAnchorageType == AnchorageType.MobileBracket) {
				btnPosition.SetTitle (NSBundle.MainBundle.LocalizedString ("Vernacular_P0_anchorage_bracket", null), UIControlState.Normal);
				btnPosition.SetImage (UIImage.FromBundle ("icon_bracket"), UIControlState.Normal);
			} else if (Settings.LastAnchorageType == AnchorageType.MobileMat) {
				btnPosition.SetTitle (NSBundle.MainBundle.LocalizedString ("Vernacular_P0_anchorage_mat", null), UIControlState.Normal);
				btnPosition.SetImage (UIImage.FromBundle ("icon_mat"), UIControlState.Normal);
			} else if (Settings.LastAnchorageType == AnchorageType.Pocket) {
				btnPosition.SetTitle (NSBundle.MainBundle.LocalizedString ("Vernacular_P0_anchorage_pocket", null), UIControlState.Normal);
				btnPosition.SetImage (UIImage.FromBundle ("icon_pocket"), UIControlState.Normal);
			}

			// Vehicle type
			if (Settings.LastVehicleType == VehicleType.Motorcycle) {
				btnVehicle.SetTitle (NSBundle.MainBundle.LocalizedString ("Vernacular_P0_vehicle_motorcycle", null), UIControlState.Normal);
				btnVehicle.SetImage (UIImage.FromBundle ("icon_motorcycle"), UIControlState.Normal);
			} else if (Settings.LastVehicleType == VehicleType.Car ) {
				btnVehicle.SetTitle (NSBundle.MainBundle.LocalizedString ("Vernacular_P0_vehicle_car", null), UIControlState.Normal);
				btnVehicle.SetImage (UIImage.FromBundle ("icon_car"), UIControlState.Normal);
			} else if (Settings.LastVehicleType == VehicleType.Truck) {
				btnVehicle.SetTitle (NSBundle.MainBundle.LocalizedString ("Vernacular_P0_vehicle_truck", null), UIControlState.Normal);
				btnVehicle.SetImage (UIImage.FromBundle ("icon_bus"), UIControlState.Normal);
			}
		}

	}
}

