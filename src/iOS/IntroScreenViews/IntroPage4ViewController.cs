
using System;

using Foundation;
using UIKit;
using SmartRoadSense.Shared;

namespace SmartRoadSense.iOS
{
	public partial class IntroPage4ViewController : UIViewController
	{
		public IntroPage4ViewController () : base ("IntroPage4ViewController", null)
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
			String title =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_4_title", null).PrepareForLabel ();
			lblTitle.Text = title;

			String body1 =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_4_question", null).PrepareForLabel ();
			lblBody1.Text = body1;	

			String matString =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_anchorage_mat", null).PrepareForLabel ();
			String staffString =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_anchorage_bracket", null).PrepareForLabel ();
			String otherString =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_anchorage_pocket", null).PrepareForLabel ();

			lblBody2.Text = staffString;

			String bottom =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_bottom_question_notice", null).PrepareForLabel ();
			lblBottom.Text = bottom;	

			// Set UI elements
			View.BackgroundColor = StyleSettings.ThemePrimaryDarkLightenedColor();
			lblTitle.TextColor = StyleSettings.ThemePrimaryColor ();
			lblBody1.TextColor = StyleSettings.TextOnDarkColor ();
			lblBody2.TextColor = StyleSettings.ThemePrimaryColor ();
			lblBottom.TextColor = StyleSettings.SubduedTextOnDarkColor ();
			btnMat.SetTitle ("", UIControlState.Normal);
			btnStaff.SetTitle ("", UIControlState.Normal);
			btnOther.SetTitle ("", UIControlState.Normal);

			// Set button backgrounds
			btnStaff.ClipsToBounds = true;
			btnStaff.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnStaff.SetBackgroundImage (ChangeImageColor.GetColoredImage ("icon_bracket", StyleSettings.ThemePrimaryColor ()), UIControlState.Normal);
			btnMat.ClipsToBounds = true;
			btnMat.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnMat.SetBackgroundImage (UIImage.FromBundle("icon_mat"), UIControlState.Normal);
			btnOther.ClipsToBounds = true;
			btnOther.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnOther.SetBackgroundImage (UIImage.FromBundle("icon_pocket"), UIControlState.Normal);

			// Button handlers

			// car button
			btnMat.TouchUpInside += delegate {
				btnMat.SetBackgroundImage (ChangeImageColor.GetColoredImage ("icon_mat", StyleSettings.ThemePrimaryColor ()), UIControlState.Normal);
				btnStaff.SetBackgroundImage (UIImage.FromBundle("icon_bracket"), UIControlState.Normal);
				btnOther.SetBackgroundImage (UIImage.FromBundle("icon_pocket"), UIControlState.Normal);

				lblBody2.Text = matString;
				selectAnchorage(AnchorageType.MobileMat);
			};

			// motorcycle button
			btnStaff.TouchUpInside += delegate {
				btnStaff.SetBackgroundImage (ChangeImageColor.GetColoredImage ("icon_bracket", StyleSettings.ThemePrimaryColor ()), UIControlState.Normal);
				btnMat.SetBackgroundImage (UIImage.FromBundle("icon_mat"), UIControlState.Normal);
				btnOther.SetBackgroundImage (UIImage.FromBundle("icon_pocket"), UIControlState.Normal);

				lblBody2.Text = staffString;	
				selectAnchorage(AnchorageType.MobileBracket);
			};

			// truck button
			btnOther.TouchUpInside += delegate {
				btnOther.SetBackgroundImage (ChangeImageColor.GetColoredImage ("icon_pocket", StyleSettings.ThemePrimaryColor ()), UIControlState.Normal);
				btnStaff.SetBackgroundImage (UIImage.FromBundle("icon_bracket"), UIControlState.Normal);
				btnMat.SetBackgroundImage (UIImage.FromBundle("icon_mat"), UIControlState.Normal);

				lblBody2.Text = otherString;
				selectAnchorage(AnchorageType.Pocket);
			};
		}

		private void selectAnchorage(AnchorageType type){
			Settings.LastAnchorageType = type;
		}
	}
}

