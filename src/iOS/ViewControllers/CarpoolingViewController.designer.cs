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
	[Register ("CarpoolingViewController")]
	partial class CarpoolingViewController
	{
		[Outlet]
		UIKit.UIButton btnBlaBla { get; set; }

		[Outlet]
		UIKit.UIButton btnContinue { get; set; }

		[Outlet]
		UIKit.UILabel lblBody1 { get; set; }

		[Outlet]
		UIKit.UILabel lblBody2 { get; set; }

		[Outlet]
		UIKit.UILabel lblBody3 { get; set; }

		[Outlet]
		UIKit.UILabel lblTitle { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (lblBody1 != null) {
				lblBody1.Dispose ();
				lblBody1 = null;
			}

			if (lblBody2 != null) {
				lblBody2.Dispose ();
				lblBody2 = null;
			}

			if (lblBody3 != null) {
				lblBody3.Dispose ();
				lblBody3 = null;
			}

			if (btnBlaBla != null) {
				btnBlaBla.Dispose ();
				btnBlaBla = null;
			}

			if (btnContinue != null) {
				btnContinue.Dispose ();
				btnContinue = null;
			}
		}
	}
}
