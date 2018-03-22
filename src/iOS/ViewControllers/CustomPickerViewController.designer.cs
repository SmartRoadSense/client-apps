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
    [Register ("CustomPickerViewController")]
    partial class CustomPickerViewController
    {
        [Outlet]
        UIKit.UIView backgroundView { get; set; }


        [Outlet]
        UIKit.UIButton btnCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnOk { get; set; }


        [Outlet]
        UIKit.UIView horView { get; set; }


        [Outlet]
        UIKit.UIPickerView pickerView { get; set; }


        [Outlet]
        UIKit.UIView vertView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (backgroundView != null) {
                backgroundView.Dispose ();
                backgroundView = null;
            }

            if (btnCancel != null) {
                btnCancel.Dispose ();
                btnCancel = null;
            }

            if (btnOk != null) {
                btnOk.Dispose ();
                btnOk = null;
            }

            if (horView != null) {
                horView.Dispose ();
                horView = null;
            }

            if (pickerView != null) {
                pickerView.Dispose ();
                pickerView = null;
            }

            if (vertView != null) {
                vertView.Dispose ();
                vertView = null;
            }
        }
    }
}