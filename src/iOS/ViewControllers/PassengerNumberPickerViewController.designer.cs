// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SmartRoadSense.iOS
{
	[Register ("PassengerNumberPickerViewController")]
	partial class PassengerNumberPickerViewController
	{
		[Outlet]
		UIKit.UIButton btnGo { get; set; }

		[Outlet]
		UIKit.UIImageView imgCarSharingLogo { get; set; }

		[Outlet]
		UIKit.UILabel lblDescription { get; set; }

		[Outlet]
		UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		UIKit.UIPickerView pickerView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (lblDescription != null) {
				lblDescription.Dispose ();
				lblDescription = null;
			}

			if (imgCarSharingLogo != null) {
				imgCarSharingLogo.Dispose ();
				imgCarSharingLogo = null;
			}

			if (pickerView != null) {
				pickerView.Dispose ();
				pickerView = null;
			}

			if (btnGo != null) {
				btnGo.Dispose ();
				btnGo = null;
			}
		}
	}
}
