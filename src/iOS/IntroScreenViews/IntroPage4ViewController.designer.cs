// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SmartRoadSense.iOS
{
    [Register ("IntroPage4ViewController")]
    partial class IntroPage4ViewController
    {
        [Outlet]
        UIKit.UIButton btnMat { get; set; }


        [Outlet]
        UIKit.UIButton btnOther { get; set; }


        [Outlet]
        UIKit.UIButton btnStaff { get; set; }


        [Outlet]
        UIKit.UILabel lblBody1 { get; set; }


        [Outlet]
        UIKit.UILabel lblBody2 { get; set; }


        [Outlet]
        UIKit.UILabel lblBottom { get; set; }


        [Outlet]
        UIKit.UILabel lblTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnMat != null) {
                btnMat.Dispose ();
                btnMat = null;
            }

            if (btnOther != null) {
                btnOther.Dispose ();
                btnOther = null;
            }

            if (btnStaff != null) {
                btnStaff.Dispose ();
                btnStaff = null;
            }

            if (lblBody1 != null) {
                lblBody1.Dispose ();
                lblBody1 = null;
            }

            if (lblBody2 != null) {
                lblBody2.Dispose ();
                lblBody2 = null;
            }

            if (lblBottom != null) {
                lblBottom.Dispose ();
                lblBottom = null;
            }

            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
            }
        }
    }
}