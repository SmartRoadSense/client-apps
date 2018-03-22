
using System;

using Foundation;
using UIKit;
using SmartRoadSense.Shared;

namespace SmartRoadSense.iOS
{
	public partial class IntroPage2ViewController : UIViewController
	{
		public IntroPage2ViewController () : base ("IntroPage2ViewController", null)
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
			String title =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_2_title", null).PrepareForLabel ();
			lblTitle.Text = title;

			String body1 =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_2_text_1", null).PrepareForLabel ();
			lblbody1.Text = body1;	

			String body2 =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_2_text_2", null).PrepareForLabel ();
			lblbody2.Text = body2;	
		
			// Set UI elements
			View.BackgroundColor = StyleSettings.ThemePrimaryDarkLightenedColor();
			lblTitle.TextColor = StyleSettings.ThemePrimaryColor ();
			lblbody1.TextColor = StyleSettings.TextOnDarkColor ();
			lblbody2.TextColor = StyleSettings.TextOnDarkColor ();


		}
	}
}

