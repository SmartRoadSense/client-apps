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
    [Register ("iOSViewController")]
    partial class iOSViewController
    {
        [Outlet]
        UIKit.NSLayoutConstraint alertViewTopConstraint { get; set; }


        [Outlet]
        UIKit.UIView bottomView { get; set; }


        [Outlet]
        UIKit.UIButton btnStart { get; set; }


        [Outlet]
        UIKit.UIButton btnStop { get; set; }


        [Outlet]
        UIKit.UIButton btnStop2 { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint btnStopBottomConstraint { get; set; }


        [Outlet]
        UIKit.UIButton btnSupport { get; set; }


        [Outlet]
        UIKit.UIButton btnVehicle { get; set; }


        [Outlet]
        UIKit.UIImageView imgError { get; set; }


        [Outlet]
        UIKit.UILabel lblBody { get; set; }


        [Outlet]
        UIKit.UILabel lblCalibration { get; set; }


        [Outlet]
        UIKit.UILabel lblCenter { get; set; }


        [Outlet]
        UIKit.UILabel lblLeft { get; set; }


        [Outlet]
        UIKit.UILabel lblRight { get; set; }


        [Outlet]
        UIKit.UILabel lblTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (alertViewTopConstraint != null) {
                alertViewTopConstraint.Dispose ();
                alertViewTopConstraint = null;
            }

            if (bottomView != null) {
                bottomView.Dispose ();
                bottomView = null;
            }

            if (btnStart != null) {
                btnStart.Dispose ();
                btnStart = null;
            }

            if (btnStop != null) {
                btnStop.Dispose ();
                btnStop = null;
            }

            if (btnStopBottomConstraint != null) {
                btnStopBottomConstraint.Dispose ();
                btnStopBottomConstraint = null;
            }

            if (btnSupport != null) {
                btnSupport.Dispose ();
                btnSupport = null;
            }

            if (btnVehicle != null) {
                btnVehicle.Dispose ();
                btnVehicle = null;
            }

            if (imgError != null) {
                imgError.Dispose ();
                imgError = null;
            }

            if (lblBody != null) {
                lblBody.Dispose ();
                lblBody = null;
            }

            if (lblCalibration != null) {
                lblCalibration.Dispose ();
                lblCalibration = null;
            }

            if (lblCenter != null) {
                lblCenter.Dispose ();
                lblCenter = null;
            }

            if (lblLeft != null) {
                lblLeft.Dispose ();
                lblLeft = null;
            }

            if (lblRight != null) {
                lblRight.Dispose ();
                lblRight = null;
            }

            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
            }
        }
    }
}