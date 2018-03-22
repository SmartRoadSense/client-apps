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
    [Register ("SideMenuController")]
    partial class SideMenuController
    {
        [Outlet]
        UIKit.UILabel lblTitle { get; set; }


        [Outlet]
        UIKit.UITableView tableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
            }

            if (tableView != null) {
                tableView.Dispose ();
                tableView = null;
            }
        }
    }
}