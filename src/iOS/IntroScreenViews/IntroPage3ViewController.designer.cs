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
    [Register ("IntroPage3ViewController")]
    partial class IntroPage3ViewController
    {
        [Outlet]
        UIKit.UIButton btnCar { get; set; }


        [Outlet]
        UIKit.UIButton btnMotorcycle { get; set; }


        [Outlet]
        UIKit.UIButton btnTruck { get; set; }


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
            if (btnCar != null) {
                btnCar.Dispose ();
                btnCar = null;
            }

            if (btnMotorcycle != null) {
                btnMotorcycle.Dispose ();
                btnMotorcycle = null;
            }

            if (btnTruck != null) {
                btnTruck.Dispose ();
                btnTruck = null;
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