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
    [Register ("IntroPageCalibrationViewController")]
    partial class IntroPageCalibrationViewController
    {
        [Outlet]
        UIKit.UIButton btnCalibrate { get; set; }


        [Outlet]
        UIKit.UILabel lblBody1 { get; set; }


        [Outlet]
        UIKit.UILabel lblBody2 { get; set; }


        [Outlet]
        UIKit.UILabel lblTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnCalibrate != null) {
                btnCalibrate.Dispose ();
                btnCalibrate = null;
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
        }
    }
}