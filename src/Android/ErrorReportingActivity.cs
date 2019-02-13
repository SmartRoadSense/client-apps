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

namespace SmartRoadSense.Android {
    
    [Activity(
        Label = "@string/Vernacular_P0_title_error_reporting",
        ParentActivity = typeof(MainActivity)
    )]
    public class ErrorReportingActivity : AppCompatActivity {

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            if (!ErrorReporter.HasDump()) {
                Toast.MakeText(this, Resource.String.Vernacular_P0_error_reporting_no_dump, ToastLength.Long).Show();

                this.Finish();
                return;
            }

            SetContentView(Resource.Layout.activity_error_reporting);

            //Toolbar support
            var toolbar = this.FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if(toolbar != null) {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            if(Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                FindViewById(Resource.Id.toolbar_shadow).Visibility = ViewStates.Gone;
            }

            FindViewById<TextView>(Resource.Id.text_introduction).ReloadTextAsHtml();
            FindViewById<TextView>(Resource.Id.text_information).ReloadTextAsHtml();

            FindViewById<Button>(Resource.Id.button_send).Click += HandleSendClick;
            FindViewById<Button>(Resource.Id.button_drop).Click += HandleDropClick;
        }

        private async void HandleSendClick(object sender, EventArgs e) {
            var dumpContent = await ErrorReporter.ReadDumpAndDelete();
            
            this.StartSendEmail(
                GetString(Resource.String.Vernacular_P0_error_reporting_send_report_chooser),
                GetString(Resource.String.Vernacular_P0_mail_address),
                GetString(Resource.String.Vernacular_P0_error_reporting_mail_subject),
                dumpContent
            );

            this.Finish();
        }

        private async void HandleDropClick(object sender, EventArgs e) {
            await ErrorReporter.DropDump();

            this.Finish();
        }

    }
}

