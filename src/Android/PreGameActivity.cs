using System;
using Android.App;
using Android.OS;
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
        ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Landscape
    )]
    public class PreGameActivity : AppCompatActivity {

        UrhoSurfacePlaceholder surface;
        Urho.Application app;


        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

            SetContentView(SmartRoadSense.Android.Resource.Layout.pregame);
            LaunchGame();           
        }

        async void LaunchGame() {
            var mLayout = new RelativeLayout(this);
            surface = UrhoSurface.CreateSurface(this);// (this, , true);
            mLayout.AddView(surface);
            SetContentView(mLayout);

            app = await surface.Show<Game>(new ApplicationOptions("Data"));
            app.Update += (obj) => {
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
