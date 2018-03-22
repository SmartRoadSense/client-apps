using System;
using Foundation;
using SmartRoadSense.Shared;
using UIKit;

namespace SmartRoadSense.iOS
{
    public partial class CarpoolingViewController : UIViewController
    {
		private const string BlaBlaCarPackageName = "com.comuto";
        public iOSViewController parentVC;

		public CarpoolingViewController(IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            lblTitle.Text = NSBundle.MainBundle.LocalizedString("Vernacular_P0_dialog_blablacar_title", null);
            lblTitle.TextColor = StyleSettings.TextOnDarkColor();

            lblBody1.Text = NSBundle.MainBundle.LocalizedString("Vernacular_P0_dialog_blablacar_description", null).PrepareForLabel();
			lblBody1.TextColor = StyleSettings.TextOnDarkColor();

			lblBody2.Text = "";
			lblBody2.TextColor = StyleSettings.TextOnDarkColor();

			lblBody3.Text = NSBundle.MainBundle.LocalizedString("Vernacular_P0_dialog_blablacar_action_hint", null);
			lblBody3.TextColor = StyleSettings.TextOnDarkColor();

            btnContinue.SetTitle(NSBundle.MainBundle.LocalizedString("Vernacular_P0_dialog_blablacar_ignore", null).ToUpper(), UIControlState.Normal);
            btnContinue.SetTitleColor(StyleSettings.TextOnDarkColor(), UIControlState.Normal);
            btnContinue.BackgroundColor = StyleSettings.ThemePrimaryColor();

            btnBlaBla.BackgroundColor = StyleSettings.LightGrayPressedColor();

            btnBlaBla.TouchUpInside += (sender, e) => {
                // Open app if available, else website
                this.DismissViewController(true, () =>
                {
                    OpenBlaBlaCar();
					parentVC.StartRecordingCommands();
				});
			};

            btnContinue.TouchUpInside += (sender, e) => {
                // Start registration
                this.DismissViewController(true, parentVC.StartRecordingCommands);
            };
		}

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void OpenBlaBlaCar()
        {
			NSUrl request = new NSUrl("blablacar://home");

			try
			{
                if (!UIApplication.SharedApplication.CanOpenUrl(request))
                {
					request = new NSUrl("itms://itunes.apple.com/app/blablacar-trusted-carpooling/id341329033");
				} 

                Log.Debug("trying to open request url {0}", request.AbsoluteString);
				if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
					UIApplication.SharedApplication.OpenUrlAsync(request, new UIApplicationOpenUrlOptions());
				else
					UIApplication.SharedApplication.OpenUrl(request);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Cannot open url: {0}, Error: {1}", request.AbsoluteString, ex.Message);
			}
        }
    }
}

