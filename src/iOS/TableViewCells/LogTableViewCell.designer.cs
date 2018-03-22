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
    [Register ("LogTableViewCell")]
    partial class LogTableViewCell
    {
        [Outlet]
        UIKit.UIImageView imgAlert { get; set; }


        [Outlet]
        UIKit.UILabel lblText { get; set; }


        [Outlet]
        UIKit.UILabel lblTime { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (imgAlert != null) {
                imgAlert.Dispose ();
                imgAlert = null;
            }

            if (lblText != null) {
                lblText.Dispose ();
                lblText = null;
            }

            if (lblTime != null) {
                lblTime.Dispose ();
                lblTime = null;
            }
        }
    }
}