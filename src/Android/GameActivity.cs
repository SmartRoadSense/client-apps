using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;

namespace SmartRoadSense.Android {

    [Activity(
        Label = "@string/Vernacular_P0_title_game",
        ParentActivity = typeof(MainActivity)
    )]
    public class GameActivity : AppCompatActivity {

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_game);

            //Toolbar support
            var toolbar = this.FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null) {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                FindViewById(Resource.Id.toolbar_shadow).Visibility = ViewStates.Gone;
            }
        }

    }
}