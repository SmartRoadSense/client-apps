using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SmartRoadSense.Shared;
using Urho;
using Urho.Droid;

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

            Window.AddFlags(WindowManagerFlags.Fullscreen);
            SetContentView(SmartRoadSense.Android.Resource.Layout.activity_pregame);
            _ = LaunchGame();
        }

        private async Task LaunchGame() {
            var mLayout = new RelativeLayout(this);
            surface = UrhoSurface.CreateSurface(this);
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

        public override void OnBackPressed() {
            //base.OnBackPressed();
            System.Diagnostics.Debug.WriteLine("back pressed");
        }

        protected override void OnResume() {
            UrhoSurface.OnResume();
            base.OnResume();
        }

        protected override void OnPause() {
            UrhoSurface.OnPause();
            base.OnPause();
        }

        public override void OnLowMemory() {
            UrhoSurface.OnLowMemory();
            base.OnLowMemory();
        }

        protected override void OnDestroy() {
            UrhoSurface.OnDestroy();
            base.OnDestroy();
        }

        public override bool DispatchKeyEvent(KeyEvent e) {
            if(!UrhoSurface.DispatchKeyEvent(e)) {
                return false;
            }

            return base.DispatchKeyEvent(e);
        }

        public override void OnWindowFocusChanged(bool hasFocus) {
            UrhoSurface.OnWindowFocusChanged(hasFocus);
            base.OnWindowFocusChanged(hasFocus);
        }

    }
}
