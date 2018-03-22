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
    [Register ("IntroPage2ViewController")]
    partial class IntroPage2ViewController
    {
        [Outlet]
        UIKit.UILabel lblbody1 { get; set; }


        [Outlet]
        UIKit.UILabel lblbody2 { get; set; }


        [Outlet]
        UIKit.UILabel lblTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblbody1 != null) {
                lblbody1.Dispose ();
                lblbody1 = null;
            }

            if (lblbody2 != null) {
                lblbody2.Dispose ();
                lblbody2 = null;
            }

            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
            }
        }
    }
}