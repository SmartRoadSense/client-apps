using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SmartRoadSense.Shared;

namespace SmartRoadSense.Android {

    public class DialogBlaBlaCar : DialogFragment {

        private const string BlaBlaCarPackageName = "com.comuto";

        public override Dialog OnCreateDialog(Bundle savedInstanceState) {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.dialog_blablacar, null, false);
            view.FindViewById<TextView>(Resource.Id.text_description).ReloadTextAsHtml();
            view.FindViewById<Button>(Resource.Id.button_continue).Click += (sender, e) => {
                Dismiss();
            };
            view.FindViewById<View>(Resource.Id.button_install).Click += (sender, e) => {
                Log.Debug("Open BlaBlaCar app in Store");

                try {
                    var i = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse("market://details?id=" + BlaBlaCarPackageName));
                    i.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
                    Activity.StartActivity(i);
                }
                catch (Exception) {
                    try {
                        var iWeb = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=" + BlaBlaCarPackageName));
                        iWeb.AddFlags(ActivityFlags.NewTask);
                        Activity.StartActivity(iWeb);
                    }
                    catch(Exception ex) {
                        Log.Error(ex, "Failed to open BlaBlaCar app");
                    }
                }

                Dismiss();
            };

            return new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.Vernacular_P0_dialog_blablacar_title)
                .SetView(view)
                .Create();
        }

        public override void OnDismiss(IDialogInterface dialog) {
            base.OnDismiss(dialog);

            OnClosed();
        }

        public event EventHandler Closed;

        protected virtual void OnClosed() {
            Closed?.Invoke(this, EventArgs.Empty);
        }

    }

}
