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
    [Register ("DataViewController")]
    partial class DataViewController
    {
        [Outlet]
        UIKit.UIButton btnPushData { get; set; }


        [Outlet]
        UIKit.UILabel lblNoData { get; set; }


        [Outlet]
        UIKit.UITableView tableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnPushData != null) {
                btnPushData.Dispose ();
                btnPushData = null;
            }

            if (lblNoData != null) {
                lblNoData.Dispose ();
                lblNoData = null;
            }

            if (tableView != null) {
                tableView.Dispose ();
                tableView = null;
            }
        }
    }
}