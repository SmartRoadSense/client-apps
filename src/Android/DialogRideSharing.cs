using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SmartRoadSense.Shared;

namespace SmartRoadSense.Android {

    public class DialogRideSharing : DialogFragment {

        public override Dialog OnCreateDialog(Bundle savedInstanceState) {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.dialog_ride_sharing, null, false);
            view.FindViewById<Button>(Resource.Id.button_1).Click += (sender, e) => {
                HandleClick(1);
            };
            view.FindViewById<Button>(Resource.Id.button_2).Click += (sender, e) => {
                HandleClick(2);
            };
            view.FindViewById<Button>(Resource.Id.button_3).Click += (sender, e) => {
                HandleClick(3);
            };
            view.FindViewById<Button>(Resource.Id.button_4).Click += (sender, e) => {
                HandleClick(4);
            };
            view.FindViewById<Button>(Resource.Id.button_5).Click += (sender, e) => {
                HandleClick(5);
            };

            return new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.Vernacular_P0_dialog_ride_sharing_title)
                .SetView(view)
                .Create();
        }

        private void HandleClick(int count) {
            Log.Debug("{0} people on trip selected", count);
            Dismiss();
            OnSelected(count);
        }

        public event EventHandler<ValueEventArgs<int>> Selected;

        protected virtual void OnSelected(int count) {
            Selected?.Invoke(this, new ValueEventArgs<int>(count));
        }

    }

}