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

    public class Fragment6RideSharing : global::Android.Support.V4.App.Fragment, IDisplayAwareFragment {

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            var view = inflater.Inflate(Resource.Layout.fragment_tutorial_6_ride_sharing, container, false);

            return view;
        }

        #region IDisplayAwareFragment implementation

        public void Shown() {

        }

        public void Hidden() {

        }

        #endregion

    }

}
