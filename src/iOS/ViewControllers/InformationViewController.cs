
using System;

using Foundation;
using UIKit;
using SmartRoadSense.Shared;
using MessageUI;

namespace SmartRoadSense.iOS
{
	public partial class InformationViewController : UIViewController
	{
		public InformationViewController (IntPtr handle) : base (handle)
		{
			this.Title = NSBundle.MainBundle.LocalizedString("Vernacular_P0_title_about", null).PrepareForLabel ();
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

			// Localized texts
			String title =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_app_name", null).PrepareForLabel ();
			lblTitle.Text = title;
			lblTitle.TextColor = StyleSettings.TextOnDarkColor();

			lblVersion.Text = String.Format ("{0}.{1}", App.Version.Major, App.Version.Minor);
			lblVersion.TextColor = StyleSettings.ThemePrimaryColor ();

			String developed =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_about_developed_by", null).ToUpper ().PrepareForLabel ();
			lblDeveloped.Text = developed;
			lblDeveloped.TextColor = StyleSettings.ThemePrimaryColor ();

            String developedWithC4rs = NSBundle.MainBundle.LocalizedString("Vernacular_P0_about_c4rs", null).PrepareForLabel();
            lblDevelopedWith.Text = developedWithC4rs;
            lblDevelopedWith.TextColor = StyleSettings.TextOnDarkColor();

			String university =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_about_author", null).PrepareForLabel ();
			lblUniversity.Text = university;
			lblUniversity.TextColor = StyleSettings.TextOnDarkColor ();

			String collaborators =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_about_contributors", null).ToUpper ().PrepareForLabel ();
			lblCollaborators.Text = collaborators;
			lblCollaborators.TextColor = StyleSettings.ThemePrimaryColor ();

			String collaboratorList =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_about_authors", null).PrepareForLabel ();
			lblCollaboratorList.Text = collaboratorList;
			lblCollaboratorList.TextColor = StyleSettings.TextOnDarkColor ();

			String design =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_about_icon_design", null).ToUpper ().PrepareForLabel ();
			lblDesign.Text = design;
			lblDesign.TextColor = StyleSettings.ThemePrimaryColor ();

			String designInfo =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_about_icon_design_text", null).PrepareForLabel ();
			lblDesignInfo.Text = designInfo;
			lblDesignInfo.TextColor = StyleSettings.TextOnDarkColor ();

			String comments =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_about_feedback", null).ToUpper ().PrepareForLabel ();
			lblComments.Text = comments;
			lblComments.TextColor = StyleSettings.ThemePrimaryColor ();

			String email =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_mail_address", null).PrepareForLabel ();
			btnEmail.SetTitle (email, UIControlState.Normal);
			btnEmail.SetTitleColor (StyleSettings.TextOnDarkColor (), UIControlState.Normal);

			lblAppInfo.Text = App.ApplicationInformation + ".";
			lblAppInfo.TextColor = StyleSettings.SubtleTextOnDarkColor ();

			btnEmail.TouchUpInside += (object sender, EventArgs e) => {

				// Send Email
				MFMailComposeViewController mailController;

				if (MFMailComposeViewController.CanSendMail) {
					Log.Debug ("opening mail controller");
					// Set mail composer
					mailController = new MFMailComposeViewController ();

					// populate email
					mailController.SetToRecipients (new string[]{NSBundle.MainBundle.LocalizedString("Vernacular_P0_mail_address", null)});
					mailController.SetSubject (NSBundle.MainBundle.LocalizedString("Vernacular_P0_app_name", null));

					// activate send button
					mailController.Finished += ( object s, MFComposeResultEventArgs args) => {
						Console.WriteLine (args.Result.ToString ());
						args.Controller.DismissViewController (true, null);
					};

					// present view controller
					this.PresentViewController (mailController, true, null);
				} else {
					Log.Debug ("failed opening mail controller");

					String errorTitle =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_information_message_title_engine_error", null).PrepareForLabel ();
					String errorBody =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_error_send_mail_not_configured", null).PrepareForLabel ();

					//Create Alert
					var errorAlertController = UIAlertController.Create(errorTitle, errorBody, UIAlertControllerStyle.Alert);

					//Add Actions
					errorAlertController.AddAction(UIAlertAction.Create(NSBundle.MainBundle.LocalizedString ("Vernacular_P0_dialog_ok", null), UIAlertActionStyle.Default, null));

					//Present Alert
					PresentViewController(errorAlertController, true, null);

				}
			};
		}
	}
}

