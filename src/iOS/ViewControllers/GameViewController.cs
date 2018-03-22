using System;
using Foundation;
using SmartRoadSense.Shared;
using UIKit;

namespace SmartRoadSense.iOS
{
    public partial class GameViewController : UIViewController
    {
		public GameViewController(IntPtr handle) : base (handle)
        {
			this.Title = NSBundle.MainBundle.LocalizedString("Vernacular_P0_title_game", null).PrepareForLabel();
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
			String title = NSBundle.MainBundle.LocalizedString("Vernacular_P0_game_tagline", null).PrepareForLabel();
			lblTitle.Text = title;
			lblTitle.TextColor = StyleSettings.TextOnDarkColor();

			// Localized texts
            String footer = NSBundle.MainBundle.LocalizedString("Vernacular_P0_game_coming_soon", null).PrepareForLabel();
			lblFooter.Text = footer;
            lblFooter.TextColor = StyleSettings.TextOnDarkColor();
        }
    }
}

