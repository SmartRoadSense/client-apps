using System;
using Android.Content;
using Android.Widget;

namespace SmartRoadSense.Android {

    public static class NavigationExtensions {

        public static void StartLocationSettings(this Context context) {
            try {
                Intent locationSettingsIntent = new Intent(global::Android.Provider.Settings.ActionLocationSourceSettings);
                context.StartActivity(locationSettingsIntent);
            }
            catch(Exception) {
                Toast.MakeText(context, Resource.String.Vernacular_P0_error_location_settings, ToastLength.Short).Show();
            }
        }

        public static void StartSendEmail(this Context context,
            string pickerTitle,
            string defaultAddress,
            string subject,
            string body = null) {
            try {
                Intent i = new Intent(Intent.ActionSend);
                i.SetType("message/rfc822");

                if(!string.IsNullOrEmpty(defaultAddress))
                    i.PutExtra(Intent.ExtraEmail, new string[] { defaultAddress });
                if(!string.IsNullOrEmpty(subject))
                    i.PutExtra(Intent.ExtraSubject, subject);
                if(!string.IsNullOrEmpty(body))
                    i.PutExtra(Intent.ExtraText, body);
                
                i.AddFlags(ActivityFlags.NewTask);

                context.StartActivity(Intent.CreateChooser(i, pickerTitle));
            }
            catch(Exception) {
                Toast.MakeText(context, Resource.String.Vernacular_P0_error_send_mail, ToastLength.Short).Show();
            }
        }

        public static void OpenErrorReporting(this Context context) {
            Intent i = new Intent(context, typeof(ErrorReportingActivity));
            context.StartActivity(i);
        }

    }

}

