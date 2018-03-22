using System;
using UIKit;
using Foundation;
using ObjCRuntime;
using AudioToolbox;

namespace SmartRoadSense.iOS
{
	public class MenuTableSource : UITableViewSource
	{
		UITableView TableView;
		SideMenuController SideMenuVC;
		String cellIdentifier = "cell";
		private String _registro = NSBundle.MainBundle.LocalizedString("Vernacular_P0_menu_log", null);
		private String _gioco = NSBundle.MainBundle.LocalizedString("Vernacular_P0_menu_game", null);
		private String _dati = NSBundle.MainBundle.LocalizedString("Vernacular_P0_menu_queue", null);
		private String _statistiche = NSBundle.MainBundle.LocalizedString("Vernacular_P0_menu_stats", null);
		private String _impostazioni = NSBundle.MainBundle.LocalizedString("Vernacular_P0_menu_settings", null);
		private String _informazioni = NSBundle.MainBundle.LocalizedString("Vernacular_P0_menu_about", null);

		private string[] TableItems;

		public MenuTableSource (UITableView tableView, SideMenuController sideMenuVC)
		{
			this.TableView = tableView;
			this.SideMenuVC = sideMenuVC;
			this.TableItems  = new string[]{ _gioco, _registro, _dati, _statistiche, _impostazioni, _informazioni };
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return TableItems.Length;
		}

		public override void RowHighlighted (UITableView tableView, NSIndexPath rowIndexPath)
		{
			TableView.CellAt (rowIndexPath).TextLabel.HighlightedTextColor = UIColor.Blue;

		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			TableView.CellAt (indexPath).TextLabel.HighlightedTextColor = UIColor.Blue;

			switch (indexPath.Row) {
                case 0:
                    SideMenuVC.OpenGameVC();
                    break;
    			case 1:
    				SideMenuVC.OpenLogVC ();
    				break;
    			case 2:
    				SideMenuVC.OpenDataVC ();
    				break;
                case 3:
                    SideMenuVC.OpenStatisticsVC();
                    break;
				case 4:
					SideMenuVC.OpenSettingsVC ();
    				break;
    			case 5:
    				SideMenuVC.OpenInfoVC ();
    				break;
    			default:
    				break;
			}

			tableView.DeselectRow (indexPath, true); // iOS convention is to remove the highlight
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{

			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
			// if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
			
			cell.BackgroundColor = UIColor.Clear;
			cell.TextLabel.TextColor = UIColor.White;
			cell.TextLabel.Text = TableItems [indexPath.Row];
			cell.TextLabel.Font = UIFont.FromName ("Helvetica-Bold", 12f);
			return cell;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 44;
		}
	}
}
	