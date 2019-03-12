using System;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using SmartRoadSense.Shared;
using UIKit;
using Urho.iOS;

namespace SmartRoadSense.iOS
{
    public partial class GameViewController : UIViewController
    {
        public GameViewController(IntPtr handle) : base(handle)
        {
            this.Title = NSBundle.MainBundle.GetLocalizedString("Vernacular_P0_title_game").ToString().PrepareForLabel();
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            // Localized texts
            //String title = NSBundle.MainBundle.GetLocalizedString("Vernacular_P0_game_tagline").ToString().PrepareForLabel();
            //lblTitle.Text = title;
            //lblTitle.TextColor = StyleSettings.TextOnDarkColor();

            // Localized texts
            //String footer = NSBundle.MainBundle.GetLocalizedString("Vernacular_P0_game_coming_soon").ToString().PrepareForLabel();
            //lblFooter.Text = footer;
            //lblFooter.TextColor = StyleSettings.TextOnDarkColor();

            btnLaunchGame.TouchUpInside += (sender, e) => {
                LaunchGame();
            };
        }

        void LaunchGame()
        {
            new Game().Run();
            UIApplication.SharedApplication.SetStatusBarHidden(true, UIStatusBarAnimation.None);
        }

        #region orientation
        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.Landscape;
        }
        #endregion
    }
}

