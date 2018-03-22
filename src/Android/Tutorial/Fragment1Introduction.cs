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

namespace SmartRoadSense.Android.Tutorial {

    public class Fragment1Introduction : global::Android.Support.V4.App.Fragment {
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            return inflater.Inflate(Resource.Layout.fragment_tutorial_1_introduction, container, false);
        }

    }

}
