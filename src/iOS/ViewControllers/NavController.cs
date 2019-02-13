
using System;
using System.Drawing;

using Foundation;
using UIKit;

namespace SmartRoadSense.iOS
{
	public partial class NavController : UINavigationController
	{
		public NavController() : base((string)null, null)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Perform any additional setup after loading the view, typically from a nib.
			// Set Nav Bar Style
			this.NavigationBar.BarStyle = UIBarStyle.BlackOpaque;
			this.NavigationBar.BarTintColor = StyleSettings.ThemePrimaryColor ();
			this.NavigationBar.TintColor = UIColor.White;
			this.NavigationBar.BackgroundColor = StyleSettings.ThemePrimaryColor ();
			this.NavigationBar.Opaque = true;
			this.NavigationBar.Translucent = false;	
		}
	}
}

