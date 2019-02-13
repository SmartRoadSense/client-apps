using System;
using Foundation;
using SmartRoadSense.Shared;
using UIKit;

namespace SmartRoadSense.iOS
{
    public partial class IntroPage5ViewController : UIViewController
    {
        public IntroPage5ViewController() : base("IntroPage5ViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.

			// Set Localized Strings
			String title = NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_6_title", null).PrepareForLabel();
			lblTitle.Text = title;

			String body1 = NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_6_text", null).PrepareForLabel();
            lblText.Text = body1;

			// Set UI elements
			View.BackgroundColor = StyleSettings.ThemePrimaryDarkLightenedColor();
			lblTitle.TextColor = StyleSettings.ThemePrimaryColor();
			lblText.TextColor = StyleSettings.TextOnDarkColor();

		}

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

