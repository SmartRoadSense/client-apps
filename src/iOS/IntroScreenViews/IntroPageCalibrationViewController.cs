using System;
using UIKit;
using Foundation;
using SmartRoadSense.Shared;
using BigTed;
using SmartRoadSense.Shared.Calibration;

namespace SmartRoadSense.iOS
{

	public partial class IntroPageCalibrationViewController : UIViewController
	{
		public IntroductionPageViewController IntroVC { get; set; }
		private Calibrator _calibrator;
		private bool _calibrating = false;
		private int _failedAttempts = 0;

		public IntroPageCalibrationViewController () : base ("IntroPageCalibrationViewController", null)
		{
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

			// Set Localized Strings
			String title =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_5_title", null).PrepareForLabel ();
			lblTitle.Text = title;

			String body1 =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_5_explanation", null).PrepareForLabel ();
			lblBody1.Text = body1;	

			String btn =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_5_calibrate", null).ToUpper ().PrepareForLabel ();
			btnCalibrate.SetTitle (btn, UIControlState.Normal);		

			// Set UI elements
			View.BackgroundColor = StyleSettings.ThemePrimaryDarkLightenedColor();
			lblTitle.TextColor = StyleSettings.ThemePrimaryColor ();
			lblBody1.TextColor = StyleSettings.TextOnDarkColor ();
			lblBody2.TextColor = StyleSettings.TextOnDarkColor ();
			btnCalibrate.SetTitleColor (StyleSettings.TextOnDarkColor (), UIControlState.Normal);
			btnCalibrate.BackgroundColor = StyleSettings.ThemePrimaryColor ();
			btnCalibrate.Layer.CornerRadius = 5;

			// Init calibrator
			_calibrator = Calibrator.Create ();

			btnCalibrate.TouchUpInside += (object sender, EventArgs e) =>{
				if(_calibrating)
					return;
				_calibrating = true;
				Calibrate ();
			};
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SetUiState (Settings.CalibrationDone);
		}

		private void SetUiState(bool completed){

			String instructions =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_5_instructions", null).PrepareForLabel ();
			String confirmation =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_5_confirmation", null).PrepareForLabel ();

			new System.Threading.Thread (new System.Threading.ThreadStart (() => {
				InvokeOnMainThread (() => {
					if (!completed) {
						Log.Debug ("calibration has not completed");
						lblBody2.Text = instructions;	
						btnCalibrate.Hidden = false;
						btnCalibrate.Enabled = true;
					} else {
						Log.Debug ("calibration has completed");
						lblBody2.Text = confirmation;
						lblBody2.Font = UIFont.BoldSystemFontOfSize (16.0f);
						lblBody2.TextColor = StyleSettings.LightGrayColor ();
						btnCalibrate.Hidden = true;
						btnCalibrate.Enabled = false;
					}
				});
			})).Start ();
		}
			
		private async void Calibrate() {
			String title = NSBundle.MainBundle.LocalizedString("Vernacular_P0_dialog_calibration_title", null).PrepareForLabel ();
			String calibrating =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_dialog_calibration_description", null).PrepareForLabel ();

			// Show progress
			BTProgressHUD.Show (title + " " + calibrating);

			var result = await _calibrator.Calibrate ();
			CalibrationTerminated (result);
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
				SetUiState(Settings.CalibrationDone);

				#if DEBUG
				//Create Alert
				var errorAlertController = UIAlertController.Create("CALIBRATION INFO", string.Format("SCALE FACTOR: {0:F3}", Settings.CalibrationScaleFactor), UIAlertControllerStyle.Alert);

				//Add Actions
				errorAlertController.AddAction(UIAlertAction.Create("ok", UIAlertActionStyle.Default, null));

				//Present Alert
				PresentViewController(errorAlertController, true, null);
				#endif
			}
			_calibrating = false;
		}
	}
}

