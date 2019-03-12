
using System;

using Foundation;
using UIKit;
using SmartRoadSense.Shared;
using System.Collections.Generic;
using CoreGraphics;
using SmartRoadSense.Core;
using System.Security.Cryptography;

namespace SmartRoadSense.iOS
{
	public partial class DiaryViewController : UIViewController
	{
		DiaryTableSource tableSource;
		UIBarButtonItem[] barButtonItems = new UIBarButtonItem[2];

		public DiaryViewController (IntPtr handle) : base (handle)
		{
            this.Title = NSBundle.MainBundle.GetLocalizedString ("Vernacular_P0_menu_log", null);
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

			// Init navigation items
			UIBarButtonItem update = new UIBarButtonItem (
				UIBarButtonSystemItem.Refresh,
				(s, e) => {
					RefreshLog ();
				}
			);

			UIBarButtonItem trash = new UIBarButtonItem (
				UIBarButtonSystemItem.Trash,
				(s, e) => {
					// Empty log
					UserLog.Clear();
					RefreshLog ();
				}
			);

			// Add button to item array
			barButtonItems[0] = update;
			barButtonItems[1] = trash;

			// add navigation items to navigation bar
			NavigationItem.RightBarButtonItems = barButtonItems;

			// set data handler
			UserLog.NewEntryAdded += HandleUserLogNewEntry;

			// Perform any additional setup after loading the view, typically from a nib.
			tableView.SeparatorColor = StyleSettings.SubtleTextOnDarkColor ();
			//tableView.SetContentOffset (new CGPoint(0, float.MaxValue), false);
			UIView view = new UIView (new CGRect (0, 0, 1, 1));
			tableView.TableFooterView = view;

			RefreshLog ();
		}

		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();

			// unset data handler
			UserLog.NewEntryAdded -= HandleUserLogNewEntry;
		}
			
		private void RefreshLog() {
			// set table source
			tableSource = new DiaryTableSource(tableView, UserLog.Entries);
			tableView.Source = tableSource;
			if (UserLog.Entries.Count > 0) {
				tableView.ReloadData ();
				tableView.Hidden = false;
			} else
				tableView.Hidden = true;
			 
			NSIndexPath ipath = NSIndexPath.FromRowSection (0, 0);

			if(tableView.VisibleCells.Length > 0)
				tableView.ScrollToRow (ipath, UITableViewScrollPosition.Top, false);

			tableView.Transform = CGAffineTransform.MakeRotation (-(float)(Math.PI));

			// update log entries count
			lblLogEntries.Text = string.Format (NSBundle.MainBundle.LocalizedString ("Vernacular_P0_log_entries_count", null), UserLog.Count);
		}

		private void HandleUserLogNewEntry(object sender, UserLog.NewEntryEventArgs e) {
			// update entries
			this.AddEntry (e.Entry);
		}

		public void AddEntry(UserLog.LogEntry entry) {
			tableSource.updateData (entry);
			RefreshLog ();
		}

	}
}

