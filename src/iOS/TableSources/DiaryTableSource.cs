using System;
using UIKit;
using Foundation;
using SmartRoadSense.Shared;
using ObjCRuntime;
using System.Collections.Generic;
using System.Collections;
using CoreGraphics;
using System.Linq;

namespace SmartRoadSense.iOS
{
	public class DiaryTableSource : UITableViewSource
	{
		UITableView TableView;
		List<UserLog.LogEntry> data;
		String cellIdentifier = "LogTableViewCell";

		public DiaryTableSource (UITableView tableView, IEnumerable<UserLog.LogEntry> data)
		{
			this.TableView = tableView;
			this.data = new List<UserLog.LogEntry>(data.Reverse ());
		}

		public void updateData(UserLog.LogEntry entry){
			data.Add(entry);
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return data.Count;
		}
			
		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.EstimatedRowHeight = 64;
			return UITableView.AutomaticDimension;
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
			var cell = tableView.DequeueReusableCell (cellIdentifier) as LogTableViewCell;
			if (cell == null) {
				var views = NSBundle.MainBundle.LoadNib ("LogTableViewCell", tableView, null);
				cell = Runtime.GetNSObject (views.ValueAt (0)) as LogTableViewCell;		
			}

			cell.Transform = CGAffineTransform.MakeRotation ((float)Math.PI);
			cell.UpdateCell (data[indexPath.Row]);

			return cell;
		}

	}
}
