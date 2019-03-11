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
using Urho.Droid;
using Urho;
using SmartRoadSense.Shared;
using Android.Content.PM;

namespace SmartRoadSense.Android {

    [Activity(
        Label = "@string/Vernacular_P0_title_game",
        ParentActivity = typeof(MainActivity),
        ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait
    )]
    public class GameActivity : AppCompatActivity {

        UrhoSurfacePlaceholder surface;
        Game app;

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(SmartRoadSense.Android.Resource.Layout.activity_game);

            //Toolbar support
            var toolbar = this.FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if(toolbar != null) {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            if(Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                FindViewById(Resource.Id.toolbar_shadow).Visibility = ViewStates.Gone;
            }

            var comingSoon = this.FindViewById<TextView>(Resource.Id.coming_soon_text);
            comingSoon.Click += (sender, e) => {
                Intent i = new Intent(this, typeof(PreGameActivity));
                StartActivity(i);
                //LaunchGame();
            };
        }

        async void LaunchGame()
        {
            var mLayout = new RelativeLayout(this);
            surface = UrhoSurface.CreateSurface(this);// (this, , true);
            mLayout.AddView(surface);
            SetContentView(mLayout);

            app = await surface.Show<Game>(new ApplicationOptions("Data"));
            app.Update += (obj) => {
                if (app.IsClosed)
                    Console.WriteLine("app is closed");
                if (app.IsDeleted)
                    Console.WriteLine("app is deleted");
                if (app.IsExiting)
                    Console.WriteLine("app is exiting");
            };
        }

        protected override void OnResume()
        {
            Console.WriteLine("OnResume");
            base.OnResume();
            //UrhoSurface.OnResume();
        }

        protected override void OnPause()
        {
            Console.WriteLine("OnPause");
            base.OnPause();
            //UrhoSurface.OnPause();
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            //UrhoSurface.OnLowMemory();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("OnDestroy");
            base.OnDestroy();
            //UrhoSurface.OnDestroy();
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (e.Action == KeyEventActions.Up && e.KeyCode == Keycode.Back)
            {
                OnBackPressed();
                return true;
            }
            if (!UrhoSurface.DispatchKeyEvent(e))
                return false;
            return base.DispatchKeyEvent(e);
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            Console.WriteLine("OnWindowFocusChanged");
           //UrhoSurface.OnWindowFocusChanged(hasFocus);
            base.OnWindowFocusChanged(hasFocus);
        }

    }
}
