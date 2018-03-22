
using System;

using Foundation;
using UIKit;
using SmartRoadSense.Shared;
using SmartRoadSense.Shared.ViewModel;
using CoreGraphics;
using System.Threading;
using System.Drawing;

namespace SmartRoadSense.iOS
{
	public partial class DataViewController : UIViewController
	{

		protected DataTableSource tableSource;
		protected UploadQueueViewModel ViewModel { get; private set; }
		UIBarButtonItem[] barButtonItems = new UIBarButtonItem[1];
		UIActivityIndicatorView activitySpinner;

		public DataViewController (IntPtr handle) : base (handle)
		{
			this.Title = NSBundle.MainBundle.LocalizedString("Vernacular_P0_title_queue", null).PrepareForLabel ();
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

			UIBarButtonItem trash = new UIBarButtonItem (
				UIBarButtonSystemItem.Trash,
				(s, e) => {
					//Create Alert
					var title = NSBundle.MainBundle.LocalizedString("Vernacular_P0_title_queue", null).PrepareForLabel ();
					var body = NSBundle.MainBundle.LocalizedString("Vernacular_P0_dialog_queue_clear_description", null).PrepareForLabel ();
					var okCancelAlertController = UIAlertController.Create(title, body, UIAlertControllerStyle.Alert);

					//Add Actions
					var okString = NSBundle.MainBundle.LocalizedString("Vernacular_P0_dialog_queue_clear_delete_all", null).PrepareForLabel ();
					var cancelString = NSBundle.MainBundle.LocalizedString("Vernacular_P0_dialog_cancel", null).PrepareForLabel ();

					okCancelAlertController.AddAction(UIAlertAction.Create(okString, UIAlertActionStyle.Default, alert => {
						if (ViewModel != null){
							ViewModel.ClearUploadQueueCommand.Execute (null);
							RefreshList();
						}
					}
					));
					okCancelAlertController.AddAction(UIAlertAction.Create(cancelString, UIAlertActionStyle.Cancel, null));

					//Present Alert
					PresentViewController(okCancelAlertController, true, null);
				}
			);

			// Add button to item array
			barButtonItems[0] = trash;

			// add navigation items to navigation bar
			NavigationItem.RightBarButtonItems = barButtonItems;
			
			// Perform any additional setup after loading the view, typically from a nib.
			tableView.SeparatorColor = StyleSettings.SubtleTextOnDarkColor ();
			UIView view = new UIView (new CGRect (0, 0, 1, 1));
			tableView.TableFooterView = view;

			String noData =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_status_no_files_in_queue", null).PrepareForLabel ();
			lblNoData.Text = noData;

			String forceUpload =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_action_force_upload", null).PrepareForLabel ().ToUpper ();
			btnPushData.SetTitle (forceUpload, UIControlState.Normal);

			btnPushData.BackgroundColor = StyleSettings.ThemePrimaryColor ();
			btnPushData.SetTitleColor (StyleSettings.TextOnDarkColor (), UIControlState.Normal);
			btnPushData.Layer.CornerRadius = 2;

			// Initialize data
			//View model setup
			ViewModel = new UploadQueueViewModel();
			ViewModel.OnCreate();

			ViewModel.UploadQueueUpdated += HandleUploadQueueUpdated;
			ViewModel.IsUploadingChanged += HandleIsUploadingChanged;

			ViewModel.RefreshQueueCommand.Execute (null);

			// Add table source
			tableSource = new DataTableSource(this.tableView, this);
			tableSource.data = ViewModel;
			tableView.Source = tableSource;

			// handle force upload button click
			btnPushData.TouchUpInside += (object sender, EventArgs e) => {
				TryDataUpload ();
			};

			ViewModel.IsUploadingChanged += (object sender, EventArgs e) => {
				if (ViewModel.IsUploading) {
					btnPushData.Enabled = false;

					// derive the center x and y
					nfloat centerX = this.View.Frame.Width / 2;
					nfloat centerY = this.View.Frame.Height / 2;

					// create the activity spinner, center it horizontall and put it 5 points above center x
					activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
					activitySpinner.Frame = new RectangleF (
						(float)(centerX - (activitySpinner.Frame.Width / 2)) ,
						(float)(centerY - activitySpinner.Frame.Height - 20) ,
						(float)activitySpinner.Frame.Width ,
						(float)activitySpinner.Frame.Height);
					activitySpinner.AutoresizingMask = UIViewAutoresizing.All;
					this.View.AddSubview (activitySpinner);
					activitySpinner.StartAnimating ();
				} else {
					btnPushData.Enabled = true;
					activitySpinner.RemoveFromSuperview();
				}
			};
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			ViewModel.RefreshQueueCommand.Execute(null);

			RefreshUploadingUi();
		}

		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();

			if (ViewModel != null) {
				ViewModel.UploadQueueUpdated -= HandleUploadQueueUpdated;
				ViewModel.IsUploadingChanged -= HandleIsUploadingChanged;

				ViewModel.OnDestroy();
				ViewModel = null;
			}
		}

		private void HandleUploadQueueUpdated(object sender, EventArgs e) {
			RefreshList();
		}

		private void HandleIsUploadingChanged(object sender, EventArgs e) {
			RefreshUploadingUi();
		}

		private void RefreshUploadingUi() {
			btnPushData.Enabled = !ViewModel.IsUploading;
		}

		private void RefreshList() {

			if (ViewModel.UploadQueue.Count > 0) {
				Log.Debug ("Number of queue items: {0}", ViewModel.UploadQueue.Count);
				tableSource.data = ViewModel;
				tableView.ReloadData ();
				lblNoData.Hidden = true;
				tableView.Hidden = false;
				btnPushData.Hidden = false;
			}
			else {
				Log.Debug ("Queue is empty");
				tableView.ReloadData ();
				lblNoData.Hidden = false;
				tableView.Hidden = true;
				btnPushData.Hidden = true;
			}
		}

		#region data upload

		private void TryDataUpload(){
			UploadData ();
		}

		private async void UploadData(){
			SyncManager SyncManager = new SyncManager();
			var src = new CancellationTokenSource();
			var token = src.Token;
			await SyncManager.Synchronize(token);
		}

		#endregion

	}
}

