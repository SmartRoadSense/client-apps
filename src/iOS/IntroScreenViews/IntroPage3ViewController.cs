
using System;

using Foundation;
using UIKit;
using SmartRoadSense.Shared;

namespace SmartRoadSense.iOS
{
	public partial class IntroPage3ViewController : UIViewController
	{
		public IntroPage3ViewController () : base ("IntroPage3ViewController", null)
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
			
			// Set Localized Strings
			String title =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_3_title", null).PrepareForLabel ();
			lblTitle.Text = title;

			String body1 =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_3_question", null).PrepareForLabel ();
			lblBody1.Text = body1;	

			String motorcycleString =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_vehicle_motorcycle", null).PrepareForLabel ();
			String carString =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_vehicle_car", null).PrepareForLabel ();
			String truckString =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_vehicle_truck", null).PrepareForLabel ();

			lblBody2.Text = carString;

			String bottom =  NSBundle.MainBundle.LocalizedString("Vernacular_P0_tutorial_bottom_question_notice", null).PrepareForLabel ();
			lblBottom.Text = bottom;	

			// Set UI String elements
			View.BackgroundColor = StyleSettings.ThemePrimaryDarkLightenedColor();
			lblTitle.TextColor = StyleSettings.ThemePrimaryColor ();
			lblBody1.TextColor = StyleSettings.TextOnDarkColor ();
			lblBody2.TextColor = StyleSettings.ThemePrimaryColor ();
			lblBottom.TextColor = StyleSettings.SubduedTextOnDarkColor ();
			btnCar.SetTitle ("", UIControlState.Normal);
			btnMotorcycle.SetTitle ("", UIControlState.Normal);
			btnTruck.SetTitle ("", UIControlState.Normal);

			// Set button backgrounds
			btnMotorcycle.ClipsToBounds = true;
			btnMotorcycle.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnMotorcycle.SetBackgroundImage (UIImage.FromBundle("icon_motorcycle"), UIControlState.Normal);
			btnCar.ClipsToBounds = true;
			btnCar.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnCar.SetBackgroundImage (ChangeImageColor.GetColoredImage ("icon_car", StyleSettings.ThemePrimaryColor ()), UIControlState.Normal);
			btnTruck.ClipsToBounds = true;
			btnTruck.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnTruck.SetBackgroundImage (UIImage.FromBundle("icon_bus"), UIControlState.Normal);

			// Button handlers

			// car button
			btnCar.TouchUpInside += delegate {
				btnCar.SetBackgroundImage (ChangeImageColor.GetColoredImage ("icon_car", StyleSettings.ThemePrimaryColor ()), UIControlState.Normal);
				btnMotorcycle.SetBackgroundImage (UIImage.FromBundle("icon_motorcycle"), UIControlState.Normal);
				btnTruck.SetBackgroundImage (UIImage.FromBundle("icon_bus"), UIControlState.Normal);

				lblBody2.Text = carString;
				selectVehicle (VehicleType.Car);
			};

			// motorcycle button
			btnMotorcycle.TouchUpInside += delegate {
				btnMotorcycle.SetBackgroundImage (ChangeImageColor.GetColoredImage ("icon_motorcycle", StyleSettings.ThemePrimaryColor ()), UIControlState.Normal);
				btnCar.SetBackgroundImage (UIImage.FromBundle("icon_car"), UIControlState.Normal);
				btnTruck.SetBackgroundImage (UIImage.FromBundle("icon_bus"), UIControlState.Normal);

				lblBody2.Text = motorcycleString;	
				selectVehicle (VehicleType.Motorcycle);
			};

			// truck button
			btnTruck.TouchUpInside += delegate {
				btnTruck.SetBackgroundImage (ChangeImageColor.GetColoredImage ("icon_bus", StyleSettings.ThemePrimaryColor ()), UIControlState.Normal);
				btnMotorcycle.SetBackgroundImage (UIImage.FromBundle("icon_motorcycle"), UIControlState.Normal);
				btnCar.SetBackgroundImage (UIImage.FromBundle("icon_car"), UIControlState.Normal);

				lblBody2.Text = truckString;
				selectVehicle(VehicleType.Truck);
			};
		}

		private void selectVehicle(VehicleType type){
			Settings.LastVehicleType = type;
		}
	}
}

