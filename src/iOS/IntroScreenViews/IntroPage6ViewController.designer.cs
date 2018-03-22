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
	[Register ("IntroPage6ViewController")]
	partial class IntroPage6ViewController
	{
		[Outlet]
		UIKit.UIButton btnGo { get; set; }

		[Outlet]
		UIKit.UILabel lblBody1 { get; set; }

		[Outlet]
		UIKit.UILabel lblBody2 { get; set; }

		[Outlet]
		UIKit.UILabel lblCalibration { get; set; }

		[Outlet]
		UIKit.UILabel lblTitle { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnGo != null) {
				btnGo.Dispose ();
				btnGo = null;
			}

			if (lblBody1 != null) {
				lblBody1.Dispose ();
				lblBody1 = null;
			}

			if (lblBody2 != null) {
				lblBody2.Dispose ();
				lblBody2 = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (lblCalibration != null) {
				lblCalibration.Dispose ();
				lblCalibration = null;
			}
		}
	}
}
