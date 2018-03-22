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
    [Register ("AlertViewController")]
    partial class AlertViewController
    {
        [Outlet]
        UIKit.UIImageView imgIcon { get; set; }


        [Outlet]
        UIKit.UILabel lblBody { get; set; }


        [Outlet]
        UIKit.UILabel lblTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (imgIcon != null) {
                imgIcon.Dispose ();
                imgIcon = null;
            }

            if (lblBody != null) {
                lblBody.Dispose ();
                lblBody = null;
            }

            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
            }
        }
    }
}