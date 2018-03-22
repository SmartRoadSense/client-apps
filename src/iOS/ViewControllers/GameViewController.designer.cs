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
	[Register ("GameViewController")]
	partial class GameViewController
	{
		[Outlet]
		UIKit.UIImageView imgLogo { get; set; }

		[Outlet]
		UIKit.UILabel lblFooter { get; set; }

		[Outlet]
		UIKit.UILabel lblTitle { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (lblFooter != null) {
				lblFooter.Dispose ();
				lblFooter = null;
			}

			if (imgLogo != null) {
				imgLogo.Dispose ();
				imgLogo = null;
			}
		}
	}
}
