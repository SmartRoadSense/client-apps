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
            Log.Debug("Received wake broadcast intent, action: {0}", intent.Action);

            if (Settings.EnableContinuousRecording) {
                SensingService.Do(context, model => {
                    model.StartRecordingCommand.Execute(null);
                });
            }
        }

        private const int AwakeRecordingIntentId = 1234;

        /// <summary>
        /// Refreshes the receiver's installation, based on continuous mode.
        /// </summary>
        public static void Install(Context appContext) {
            var isContinuousMode = Settings.EnableContinuousRecording;
            Log.Debug("Installing boot receiver (continuous mode: {0})", isContinuousMode);

            try {
                AlarmManager manager = (AlarmManager)appContext.GetSystemService(Context.AlarmService);

                var broadcastIntent = new Intent(appContext, typeof(BootReceiver));
                var pendingIntent = PendingIntent.GetBroadcast(appContext,
                    AwakeRecordingIntentId, broadcastIntent, PendingIntentFlags.UpdateCurrent);

                manager.Cancel(pendingIntent);

                if(isContinuousMode) {
                    manager.SetInexactRepeating(
                        AlarmType.RtcWakeup,
                        DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)).ToUnixEpochMilliseconds(),
#if DEBUG
                        (long)TimeSpan.FromMinutes(1).TotalMilliseconds,
#else
                        AlarmManager.IntervalFifteenMinutes,
#endif
                        pendingIntent
                    );
                }
            }
            catch(Exception ex) {
                Log.Error(ex, "Failed to setup boot receiver for continuous mode {0}", isContinuousMode);
            }
        }

    }

}

