using System;
using System.Collections.Generic;
using System.Drawing;
using Foundation;
using SmartRoadSense.Shared;
using UIKit;

namespace SmartRoadSense.iOS
{
    public partial class PassengerNumberPickerViewController : UIViewController
    {
		IList<String> dataModel;
        public iOSViewController parentVC { get; set; }

		public PassengerNumberPickerViewController(IntPtr handle) : base (handle)
        {
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


			// set data model
			dataModel = SetDataModel();

            btnGo.SetTitle(NSBundle.MainBundle.LocalizedString("Vernacular_P0_dialog_accept", null).ToUpper(), UIControlState.Normal);
			btnGo.SetTitleColor(StyleSettings.TextOnDarkColor(), UIControlState.Normal);
			btnGo.BackgroundColor = StyleSettings.ThemePrimaryColor();

			// Set pickerview data model
			TypePickerViewModel model = new TypePickerViewModel();
			pickerView.Model = model;
			model.dataModel = this.dataModel;
			pickerView.ShowSelectionIndicator = true;

			// set default selected row

			// btn handlers
			btnGo.TouchUpInside += (object sender, EventArgs e) => {
				var selected = SetSelectedType(pickerView.SelectedRowInComponent(0));
				this.DismissViewController(true, () => {
                    Settings.LastNumberOfPeople = selected;
                    parentVC.InitRecording();
				});
			};
		}

		private int SetSelectedType(nint selection)
		{
    		Log.Debug("selected number of people: {0}", selection);
			int numPeople = 1;

			switch (selection)
			{
				case 0:
                    numPeople = 1;
					break;
				case 1:
					numPeople = 2;
					break;
				case 2:
					numPeople = 3;
					break;
                case 3:
					numPeople = 4;
                    break;
                case 4:
					numPeople = 5;
                    break;
				default:
                    numPeople = -1;
					break;
			}

            return numPeople;		
		}

		private IList<String> SetDataModel()
		{
				List<String> data = new List<String> { "1", "2", "3", "4", "5+" };
				return data;
		}

		public class TypePickerViewModel : UIPickerViewModel
		{
			public IList<String> dataModel { get; set; }

			public override nint GetComponentCount(UIPickerView pickerView)
			{
				return 1;
			}

			public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
			{
				return dataModel.Count;
			}

			public override string GetTitle(UIPickerView pickerView, nint row, nint component)
			{
				return dataModel[(int)row];
			}

			public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
			{
				UILabel lbl = new UILabel(new RectangleF(0, 0, 200f, 40f));
				lbl.TextColor = StyleSettings.ThemePrimaryColor();
				lbl.TextAlignment = UITextAlignment.Center;
				lbl.Text = GetTitle(pickerView, row, component);
				return lbl;
			}
		}
    }
}

