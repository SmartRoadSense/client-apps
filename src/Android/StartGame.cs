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
            var toolbar = this.FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if(toolbar != null) {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            if(Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                FindViewById(Resource.Id.toolbar_shadow).Visibility = ViewStates.Gone;
            }
            LaunchGame();
            /*var comingSoon = this.FindViewById<TextView>(Resource.Id.coming_soon_text);
            comingSoon.Click += (sender, e) => {
                //
                //Android: orientazione dello schermo presa dalla classe madre dal quale viene lanciato
                Intent i = new Intent(this, typeof(StartGame));
                StartActivity(i);
            };*/
        }

        async void LaunchGame() {
            var mLayout = new RelativeLayout(this);
            surface = UrhoSurface.CreateSurface(this);// (this, , true);
            mLayout.AddView(surface);
            SetContentView(mLayout);
            app = await surface.Show<Game>(new ApplicationOptions("Data"));
            app.Update += (obj) => {
                Console.WriteLine("here1");
                if(app.IsClosed)
                    Console.WriteLine("app is closed");
                if(app.IsDeleted)
                    Console.WriteLine("app is deleted");
                if(app.IsExiting)
                    Console.WriteLine("app is exiting");
            };
        }

        protected override void OnResume() {
            UrhoSurface.OnResume();
            base.OnResume();
        }

        protected override void OnPause() {
            Console.WriteLine("OnPause");
            UrhoSurface.OnPause();
            base.OnPause();
        }

        public override void OnLowMemory() {
            UrhoSurface.OnLowMemory();
            base.OnLowMemory();
        }

        protected override void OnDestroy() {
            Console.WriteLine("OnDestroy");
            UrhoSurface.OnDestroy();
            ScreenOrientation UserPortrait;
            base.OnDestroy();
        }

        public override bool DispatchKeyEvent(KeyEvent e) {
            if(e.Action == KeyEventActions.Up && e.KeyCode == Keycode.Back) {
                OnBackPressed();
                return true;
            }
            if(!UrhoSurface.DispatchKeyEvent(e))
                return false;
            return base.DispatchKeyEvent(e);
        }

        public override void OnWindowFocusChanged(bool hasFocus) {
            Console.WriteLine("OnWindowFocusChanged");
            UrhoSurface.OnWindowFocusChanged(hasFocus);
            base.OnWindowFocusChanged(hasFocus);
        }

    }
}
