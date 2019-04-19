using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace SmartRoadSense.Android {

    [Activity(
        Label = "@string/Vernacular_P0_title_game",
        ParentActivity = typeof(MainActivity),
        ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait
    )]
    public class GameActivity : AppCompatActivity {

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(SmartRoadSense.Android.Resource.Layout.activity_game);

            //Toolbar support
            var toolbar = FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if(toolbar != null) {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            if(Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                FindViewById(Resource.Id.toolbar_shadow).Visibility = ViewStates.Gone;
            }

            var comingSoon = this.FindViewById<TextView>(Resource.Id.coming_soon_text);
            comingSoon.Click += (sender, e) => {
                // Stop sensing before launching the game
                SensingService.Do(model => {
                    model.StopRecordingCommand.Execute(null);
                });

                Intent i = new Intent(this, typeof(PreGameActivity));
                StartActivity(i);
            };
        }

    }
}
