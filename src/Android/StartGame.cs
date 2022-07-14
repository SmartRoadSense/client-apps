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
using AndroidX.AppCompat.App;
using SmartRoadSense.Shared;
using System.Threading.Tasks;
using Urho.Droid;
using Urho;
using Android.Content.PM;

namespace SmartRoadSense.Android {
    [Activity(
    Label = "@string/Vernacular_P0_title_game",
    ParentActivity = typeof(MainActivity),
    ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation,
    ScreenOrientation = ScreenOrientation.Landscape)]
    public class StartGame : AppCompatActivity {

        UrhoSurfacePlaceholder surface;
        Urho.Application app;

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.activity_game);

            //Toolbar support
            var toolbar = this.FindViewById<global::AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            if(toolbar != null) {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            if(Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                FindViewById(Resource.Id.toolbar_shadow).Visibility = ViewStates.Gone;
            }
        }
    }
}
