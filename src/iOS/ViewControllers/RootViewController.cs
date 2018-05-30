using System;
using System.Drawing;
using UIKit;
using iOS;

namespace SmartRoadSense.iOS
{
	public partial class RootViewController : UIViewController
	{
		// the sidebar controller for the app
		public SidebarNavigation.SidebarController SidebarController { get; private set; }

		// the navigation controller
		public NavController NavController { get; private set; }

		public RootViewController() : base(null, null)
		{

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// create a slideout navigation controller with the top navigation controller and the menu view controller
			NavController = new NavController();
			UIStoryboard storyboard = UIStoryboard.FromName ("MainStoryboard", null);
			iOSViewController iosVC = storyboard.InstantiateViewController ("iOSViewController") as iOSViewController;
			NavController.PushViewController(iosVC, false);
			SidebarController = new SidebarNavigation.SidebarController(this, NavController, new SideMenuController());
            SidebarController.MenuLocation = SidebarNavigation.MenuLocations.Left;
			SidebarController.MenuWidth = 220;
			SidebarController.ReopenOnRotate = false;
		}
	}
}

