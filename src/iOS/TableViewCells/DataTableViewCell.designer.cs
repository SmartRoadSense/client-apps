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
    [Register ("DataTableViewCell")]
    partial class DataTableViewCell
    {
        [Outlet]
        UIKit.UILabel lblDate { get; set; }


        [Outlet]
        UIKit.UILabel lblFileName { get; set; }


        [Outlet]
        UIKit.UILabel lblRecorded { get; set; }


        [Outlet]
        UIKit.UILabel lblSize { get; set; }


        [Outlet]
        UIKit.UILabel lblSizeData { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblDate != null) {
                lblDate.Dispose ();
                lblDate = null;
            }

            if (lblFileName != null) {
                lblFileName.Dispose ();
                lblFileName = null;
            }

            if (lblRecorded != null) {
                lblRecorded.Dispose ();
                lblRecorded = null;
            }

            if (lblSize != null) {
                lblSize.Dispose ();
                lblSize = null;
            }

            if (lblSizeData != null) {
                lblSizeData.Dispose ();
                lblSizeData = null;
            }
        }
    }
}