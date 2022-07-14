using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using SmartRoadSense.Shared;

namespace SmartRoadSense.Android.Tutorial {

    public class Fragment7Ready : global::AndroidX.Fragment.App.Fragment, IDisplayAwareFragment {

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            var view = inflater.Inflate(Resource.Layout.fragment_tutorial_7_ready, container, false);

            var buttonDone = view.FindViewById<Button>(Resource.Id.button_done);
            buttonDone.Click += (sender, e) => {
                Settings.DidShowTutorial = true;
                if(Activity != null)
                    Activity.Finish();
            };

            return view;
        }

        #region IDisplayAwareFragment implementation

        public void Shown() {
            if(View == null)
                return;
            
            View.FindViewById<Button>(Resource.Id.button_done).Enabled = Settings.CalibrationDone;
            View.FindViewById<TextView>(Resource.Id.text_missing_calibration).Visibility = Settings.CalibrationDone.TrueToGone();
        }

        public void Hidden() {
            
        }

        #endregion

    }

}
