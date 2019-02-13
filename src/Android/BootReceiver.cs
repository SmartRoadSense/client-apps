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

using SmartRoadSense.Shared;

namespace SmartRoadSense.Android {
    
    [BroadcastReceiver(
        Label = "BootReceiver",
        Enabled = true,
        Exported = true
    )]
    [IntentFilter(
        new string[] {
            Intent.ActionBootCompleted
        }
    )]
    public class BootReceiver : BroadcastReceiver {

        public override void OnReceive(Context context, Intent intent) {
            var action = intent.Action;

            if (action.Equals(Intent.ActionBootCompleted, StringComparison.InvariantCultureIgnoreCase)) {
                Log.Debug("Received boot completed broadcast intent");

                if (Settings.StartAtBoot) {
                    var launchIntent = new Intent(context, typeof(MainActivity));
                    launchIntent.SetAction(MainActivity.IntentStartRecording);
                    launchIntent.AddFlags(ActivityFlags.NewTask);

                    Log.Debug("Attempting to launch SmartRoadSense and start recording");
                    try {
                        context.StartActivity(launchIntent);
                    }
                    catch(Exception ex) {
                        Log.Error(ex, "Failed to launch main activity on boot completed intent");
                    }
                }
                else {
                    Log.Debug("Ignoring boot intent");
                }
            }
        }

    }

}

