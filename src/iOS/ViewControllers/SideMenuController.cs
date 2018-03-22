
using System;

using Foundation;
using UIKit;
using System.Drawing;
using SidebarNavigation;
using CoreGraphics;
using iOS;

namespace SmartRoadSense.iOS
{
	public partial class SideMenuController : BaseController
	{
		private MenuTableSource MenuTableSource;
		private GameViewController GameVC;
		private DataViewController DataVC;
		private DiaryViewController DiaryVC;
		private InformationViewController InformationVC;
		private StatisticsViewController StatisticsVC;
		private SettingsViewController SettingsVC;
		private UIStoryboard storyboard;
		private String _appName = NSBundle.MainBundle.LocalizedString("Vernacular_P0_title_main", null);

		public SideMenuController () : base ("SideMenuController", null)
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

			storyboard = UIStoryboard.FromName ("MainStoryboard", null);

			// Perform any additional setup after loading the view, typically from a nib.
			MenuTableSource = new MenuTableSource(tableView, this);
			tableView.Source = MenuTableSource;
			tableView.SeparatorColor = StyleSettings.ThemePrimaryDarkLightenedColor ();

			UIView view = new UIView (new CGRect (0, 0, 1, 1));
			tableView.TableFooterView = view;

			lblTitle.Text = _appName;
		}

		public void OpenGameVC()
		{
			GameVC = storyboard.InstantiateViewController("GameViewController") as GameViewController;
			NavController.PushViewController(GameVC, true);
			SidebarController.CloseMenu(true);
		}

		public void OpenLogVC() {
			DiaryVC = storyboard.InstantiateViewController ("DiaryViewController") as DiaryViewController;
			NavController.PushViewController (DiaryVC, true);
			SidebarController.CloseMenu (true);
		}

		public void OpenDataVC() {
			DataVC = storyboard.InstantiateViewController ("DataViewController") as DataViewController;
			NavController.PushViewController (DataVC, true);
			SidebarController.CloseMenu (true);
		}

		public void OpenStatisticsVC()
		{
			StatisticsVC = storyboard.InstantiateViewController("StatisticsViewController") as StatisticsViewController;
			NavController.PushViewController(StatisticsVC, true);
			SidebarController.CloseMenu(true);
		}

		public void OpenSettingsVC() {
			SettingsVC = storyboard.InstantiateViewController ("SettingsViewController") as SettingsViewController;
			NavController.PushViewController (SettingsVC, true);
			SidebarController.CloseMenu (true);
		}

		public void OpenInfoVC() {
			InformationVC = storyboard.InstantiateViewController ("InformationViewController") as InformationViewController;
			NavController.PushViewController (InformationVC, true);
			SidebarController.CloseMenu (true);
		}
	}
}

