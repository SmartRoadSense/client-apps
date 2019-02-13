using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;

using SmartRoadSense.Shared;

namespace SmartRoadSense.Android {

    [Activity(
        Label = "@string/Vernacular_P0_title_about",
        ParentActivity = typeof(MainActivity)
    )]
    public class AboutActivity : AppCompatActivity {

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_about);

            //Toolbar support
            var toolbar = this.FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if(toolbar != null) {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            if(Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                FindViewById(Resource.Id.toolbar_shadow).Visibility = ViewStates.Gone;
            }

#if DEBUG
            FindViewById<TextView>(Resource.Id.text_version).Text = string.Format("{0} {1}", App.Version, GetString(Resource.String.Vernacular_P0_about_version_debug).ToLower());
#elif BETA
            FindViewById<TextView>(Resource.Id.text_version).Text = string.Format("{0} {1}", App.Version, GetString(Resource.String.Vernacular_P0_about_version_beta).ToLower());
#else
            FindViewById<TextView>(Resource.Id.text_version).Text = App.Version.ToString();
#endif

            FindViewById<View>(Resource.Id.image_c4rs).Click += HandleProjectClick;

            FindViewById<TextView>(Resource.Id.text_authors).ReloadTextAsHtml();

            FindViewById<TextView>(Resource.Id.text_design).ReloadTextAsHtml();

            var buttonMail = FindViewById<View>(Resource.Id.text_feedback_mail);
            buttonMail.Clickable = true;
            buttonMail.Click += HandleFeedbackClick;

            FindViewById<TextView>(Resource.Id.text_information).Text = App.ApplicationInformation + ".";
        }

        public override bool OnOptionsItemSelected(IMenuItem item) {
            if(item == null)
                return false;

            switch (item.ItemId) {
                case global::Android.Resource.Id.Home:
                    NavUtils.NavigateUpFromSameTask(this);

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void HandleProjectClick(object sender, EventArgs e) {
            try {
                var i = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse("http://www.c4rs.eu"));
                StartActivity(i);
            }
            catch(Exception ex) {
                Log.Error(ex, "Unable to open website");
            }
        }

        private void HandleFeedbackClick(object sender, EventArgs e) {
            var address = GetString(Resource.String.Vernacular_P0_mail_address);

            this.StartSendEmail(
                GetString(Resource.String.Vernacular_P0_about_feedback_chooser),
                address,
                GetString(Resource.String.Vernacular_P0_app_name)
            );
        }

    }

}
