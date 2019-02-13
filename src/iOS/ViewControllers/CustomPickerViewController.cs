
using System;

using Foundation;
using UIKit;
using SmartRoadSense.Shared;
using System.Collections.Generic;
using System.Collections;
using WebKit;
using System.ComponentModel;
using System.Drawing;
using ObjCRuntime;

namespace SmartRoadSense.iOS
{
	public partial class CustomPickerViewController : UIViewController
	{
		public int pickerType { get; set; }
		IList<String> dataModel;
		public SettingsViewController SettingsVC { get; set; }

		public CustomPickerViewController (IntPtr handle) : base (handle)
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

			// set UI
			backgroundView.Layer.CornerRadius = 10;
			backgroundView.Layer.BorderColor = StyleSettings.LightGrayColor ().CGColor;
			backgroundView.Layer.BorderWidth = 1;
			horView.Layer.BackgroundColor = StyleSettings.LightGrayColor ().CGColor;
			vertView.Layer.BackgroundColor = StyleSettings.LightGrayColor ().CGColor;
			btnOk.SetTitle (NSBundle.MainBundle.LocalizedString ("Vernacular_P0_dialog_accept", null), UIControlState.Normal);
			btnCancel.SetTitle (NSBundle.MainBundle.LocalizedString ("Vernacular_P0_dialog_cancel", null), UIControlState.Normal);

			// set data model
			dataModel = SetDataModel(pickerType);

			// Set pickerview data model
			TypePickerViewModel model = new TypePickerViewModel();
			pickerView.Model = model;
			model.dataModel = this.dataModel;
			pickerView.ShowSelectionIndicator = true;

			// set default selected row

			// btn handlers
			btnOk.TouchUpInside += (object sender, EventArgs e) =>{
				SetSelectedType (pickerView.SelectedRowInComponent (0), this.pickerType);
				this.DismissViewController (true, () => {
					SettingsVC.SetButtonTexts ();
				});
			};

			btnCancel.TouchUpInside += (object sender, EventArgs e) => {
				this.DismissViewController (true, null);
			};
		}

		private void SetSelectedType(nint selection, int type){
			Log.Debug ("selected type: {0}", selection);
			if (pickerType == PreferencesSettings.AnchorageTypePicker) {
				switch(selection){
				case 0:
					Settings.LastAnchorageType = AnchorageType.MobileMat;
					break;
				case 1:
					Settings.LastAnchorageType = AnchorageType.MobileBracket;
					break;
				case 2:
					Settings.LastAnchorageType = AnchorageType.Pocket;
					break;
				default:
					break;
				}
			} else if (pickerType == PreferencesSettings.VehicleTypePicker) {
				switch (selection) {
				case 0:
					Settings.LastVehicleType = VehicleType.Motorcycle;
					break;
				case 1:
					Settings.LastVehicleType = VehicleType.Car;
					break;
				case 2:
					Settings.LastVehicleType = VehicleType.Truck;
					break;
				default:
					break;
				}
			} else {
				Log.Debug ("no picker type selected");
			}
		}

		private void SetNewIcon(){

		}

		private IList<String> SetDataModel(int type){
			if (pickerType == PreferencesSettings.VehicleTypePicker) {
				String motorcicle = NSBundle.MainBundle.LocalizedString("Vernacular_P0_vehicle_motorcycle", null).PrepareForLabel ();
				String car = NSBundle.MainBundle.LocalizedString("Vernacular_P0_vehicle_car", null).PrepareForLabel ();
				String bus = NSBundle.MainBundle.LocalizedString("Vernacular_P0_vehicle_truck", null).PrepareForLabel ();
				List<String> data = new List<String>{motorcicle, car, bus};
				return data;
			}
			else if (pickerType == PreferencesSettings.AnchorageTypePicker) {
				String mat = NSBundle.MainBundle.LocalizedString("Vernacular_P0_anchorage_mat", null).PrepareForLabel ();
				String bracket = NSBundle.MainBundle.LocalizedString("Vernacular_P0_anchorage_bracket", null).PrepareForLabel ();
				String pocket = NSBundle.MainBundle.LocalizedString("Vernacular_P0_anchorage_pocket", null).PrepareForLabel ();
				List<String> data = new List<String>{mat, bracket, pocket};		
				return data;
			}
			else {
				Log.Debug ("can't assign picker view model: no picker type selected");
				List<String> data = new List<String>{""};	
				return data;
			}
		}

		public class TypePickerViewModel : UIPickerViewModel
		{
			public IList<String> dataModel {get; set; }

			public override nint GetComponentCount (UIPickerView pickerView)
			{
				return 1;
			}
				
			public override nint GetRowsInComponent (UIPickerView pickerView, nint component)
			{
				return dataModel.Count;
			}

			public override string GetTitle (UIPickerView pickerView, nint row, nint component)
			{
				return dataModel[(int)row];
			}

			public override UIView GetView (UIPickerView pickerView, nint row, nint component, UIView view)
			{
				UILabel lbl = new UILabel(new RectangleF(0, 0, 200f, 40f));
				lbl.TextColor = StyleSettings.ThemePrimaryColor ();
				lbl.TextAlignment = UITextAlignment.Center;
				lbl.Text = GetTitle(pickerView, row, component);
				return lbl;			
			}

		}
	}
}

