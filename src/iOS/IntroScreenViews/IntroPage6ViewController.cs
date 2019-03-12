
using System;
using Foundation;
using UIKit;
using iOS;
using SmartRoadSense.Shared;

namespace SmartRoadSense.iOS {
    public partial class IntroPage6ViewController : UIViewController {
        public IntroductionPageViewController IntroVC { get; set; }

        public IntroPage6ViewController() : base("IntroPage6ViewController", null) {
        }

        public override void DidReceiveMemoryWarning() {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad() {
            base.ViewDidLoad();

            // Set Localized Strings
            String title = NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_7_title", null).PrepareForLabel();
            lblTitle.Text = title;

            String body1 = NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_7_text_1", null).PrepareForLabel();
            lblBody1.Text = body1;

            String body2 = NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_7_text_2", null).PrepareForLabel();
            lblBody2.Text = body2;

            String btn = NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_done", null).ToUpper().PrepareForLabel();
            btnGo.SetTitle(btn, UIControlState.Normal);

            String calibrationLabel = NSBundle.MainBundle.LocalizedString("Vernacular_P0_calibration_not_done", null);
            lblCalibration.Text = calibrationLabel;
            lblCalibration.Hidden = false;

            // Set UI elements
            View.BackgroundColor = StyleSettings.ThemePrimaryDarkLightenedColor();
            lblTitle.TextColor = StyleSettings.ThemePrimaryColor();
            lblBody1.TextColor = StyleSettings.TextOnDarkColor();
            lblBody2.TextColor = StyleSettings.TextOnDarkColor();
            lblCalibration.TextColor = StyleSettings.LightGrayColor();
            btnGo.SetTitleColor(StyleSettings.TextOnDarkColor(), UIControlState.Normal);
            btnGo.Layer.CornerRadius = 5;
#if DEBUG
            btnGo.Enabled = true;
#else
            btnGo.Enabled = false;
#endif
            btnGo.BackgroundColor = StyleSettings.LightGrayColor();

			if(Settings.CalibrationDone)
            {
                btnGo.Enabled = true;
				btnGo.BackgroundColor = StyleSettings.ThemePrimaryColor();
				lblCalibration.Hidden = true;
            }

			btnGo.TouchUpInside += (object sender, EventArgs e) => {
				if(NSUserDefaults.StandardUserDefaults.BoolForKey (PreferencesSettings.FirstLaunchKey)){
					IntroVC.DismissViewController (true, null);
				} else {
					NSUserDefaults.StandardUserDefaults.SetBool(true, PreferencesSettings.FirstLaunchKey);
					var window = UIApplication.SharedApplication.KeyWindow;
					window.RootViewController = new RootViewController ();
				}
			};
		}
	}
}

