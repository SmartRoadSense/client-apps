using System;
using UIKit;
using Foundation;
using SmartRoadSense.Shared;
using SmartRoadSense.Shared.ViewModel;
using ObjCRuntime;

namespace SmartRoadSense.iOS
{
	public class DataTableSource : UITableViewSource
	{
		UITableView TableView;
		NSString cellIdentifier = (NSString)"DataTableViewCell";
		public UploadQueueViewModel data;

		public DataTableSource (UITableView tableView, DataViewController dataViewVC)
		{
			this.TableView = tableView;
			Log.Debug ("updating table source");
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return data.UploadQueue.Count;
		}

		public override void RowHighlighted (UITableView tableView, NSIndexPath rowIndexPath)
		{
			TableView.CellAt (rowIndexPath).TextLabel.HighlightedTextColor = UIColor.Blue;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			TableView.CellAt (indexPath).TextLabel.HighlightedTextColor = UIColor.Blue;

			tableView.DeselectRow (indexPath, true); // iOS convention is to remove the highlight
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{

			var cell = tableView.DequeueReusableCell (cellIdentifier) as DataTableViewCell;
			if (cell == null) {
				var views = NSBundle.MainBundle.LoadNib ("DataTableViewCell", tableView, null);
				cell = Runtime.GetNSObject (views.ValueAt (0)) as DataTableViewCell;		
			}
			cell.UpdateCell (data.UploadQueue[indexPath.Row]);
			return cell;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 87;
		}
	}
}
