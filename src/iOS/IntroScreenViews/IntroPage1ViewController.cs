
using System;

using Foundation;
using UIKit;
using SmartRoadSense.Shared;

namespace SmartRoadSense.iOS
{
	public partial class IntroPage1ViewController : UIViewController
	{
		public IntroPage1ViewController () : base ("IntroPage1ViewController", null)
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
			String title =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_1_title", null).PrepareForLabel ();
			lblTitle.Text = title;

			String body =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_1_text_1", null).PrepareForLabel ();
			lblBody.Text = body;

			// Set UI elements
			View.BackgroundColor = StyleSettings.ThemePrimaryDarkLightenedColor();
			lblTitle.TextColor = StyleSettings.ThemePrimaryColor ();
			lblBody.TextColor = StyleSettings.TextOnDarkColor ();
		}
	}
}

