// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace SmartRoadSense.iOS
{
    [Register ("IntroPage5ViewController")]
    partial class IntroPage5ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView img { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (img != null) {
                img.Dispose ();
                img = null;
            }

            if (lblText != null) {
                lblText.Dispose ();
                lblText = null;
            }

            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
            }
        }
    }
}