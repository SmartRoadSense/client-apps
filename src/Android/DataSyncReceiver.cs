using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Net;
using Android.OS;
using AndroidX.Legacy.Content;
using SmartRoadSense.Shared;
using SmartRoadSense.Shared.Data;

namespace SmartRoadSense.Android {

    [BroadcastReceiver]
    public class DataSyncReceiver : WakefulBroadcastReceiver {

        public override void OnReceive(Context context, Intent intent) {
            Log.Debug("Received data sync broadcast request");

            Intent serviceIntent = new Intent(context, typeof(DataSyncService));
            StartWakefulService(context, serviceIntent);
        }

        /// <summary>
        /// Configures synchronization with periodic execution.
        /// </summary>
        public static void ConfigureSync(Context applicationContext) {
            Log.Debug("Configuring synchronization (last: {0}, next: {1}, deadline: {2})",
                Settings.LastUploadAttempt, SyncManager.NextUploadOpportunity, SyncManager.NextUploadDeadline);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                ConfigureSyncLegacy(applicationContext, false);
                ConfigureSyncJob(applicationContext, true);
            }
            else {
                ConfigureSyncLegacy(applicationContext, true);
            }
        }

        private const int DataSyncPendingIntentId = 1;

        /// <summary>
        /// Installs synchronization using legacy Android features (i.e. alarms).
        /// </summary>
        private static void ConfigureSyncLegacy(Context applicationContext, bool enabled) {
            Log.Debug("Configuring legacy synchronization, enabled: {0}", enabled);

            try {
                var broadcastIntent = new Intent(applicationContext, typeof(DataSyncReceiver));
                var pendingIntent = PendingIntent.GetBroadcast(applicationContext,
                    DataSyncPendingIntentId, broadcastIntent, PendingIntentFlags.UpdateCurrent);

                AlarmManager manager = (AlarmManager)applicationContext.GetSystemService(Context.AlarmService);

                manager.Cancel(pendingIntent);

                if (enabled) {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat) {
                        manager.SetWindow(
                            AlarmType.Rtc, //Deadline expressed in milliseconds from Unix epoch, does not wake up device
                            SyncManager.NextUploadOpportunity.ToUnixEpochMilliseconds(),
                            (long)((SyncManager.MaxSynchronizationInterval - SyncManager.MinSynchronizationInterval).TotalMilliseconds),
                            pendingIntent);

                        Log.Debug("Scheduled alarm on window from {0} to {1}",
                            SyncManager.NextUploadOpportunity, SyncManager.NextUploadDeadline);
                    }
                    else {
                        manager.Set(
                            AlarmType.Rtc, //Deadline expressed in milliseconds from Unix epoch, does not wake up device
                            SyncManager.NextUploadOpportunity.ToUnixEpochMilliseconds(),
                            pendingIntent);

                        Log.Debug("Scheduled alarm at {0}",
                            SyncManager.NextUploadOpportunity);
                    }
                }
            }
            catch (Exception ex) {
                Log.Error(ex, "Failed to setup synchronization on alarm manager");
            }
        }

        private const int DataSyncJobId = 1;

        /// <summary>
        /// Installs synchronization using a Job Scheduler.
        /// </summary>
        /// <remarks>
        /// Can only be used on Android Lollipop or more recent.
        /// </remarks>
        private static void ConfigureSyncJob(Context applicationContext, bool enabled) {
            Log.Debug("Configuring job scheduler synchronization, enabled: {0}", enabled);

            try {
                JobScheduler scheduler = (JobScheduler)applicationContext.GetSystemService(Context.JobSchedulerService);

                if (enabled) {
                    Log.Debug("Constructing job info, min latency {0}ms, deadline {1}ms, unmetered {2}, component {3}",
                        SyncManager.NextUploadOpportunity.MillisecondsFromNow(),
                        SyncManager.NextUploadDeadline.MillisecondsFromNow(),
                        Settings.PreferUnmeteredConnection,
                        new DataSyncJobService().ComponentName);

                    var jobInfo = new JobInfo.Builder(DataSyncJobId, new DataSyncJobService().ComponentName)
                        .SetMinimumLatency(Math.Max(SyncManager.NextUploadOpportunity.MillisecondsFromNow(), 0))
                        .SetOverrideDeadline(Math.Max(SyncManager.NextUploadDeadline.MillisecondsFromNow(), 0))
#if !DEBUG
                        .SetPersisted(true)
#endif
                        .SetRequiredNetworkType(Settings.PreferUnmeteredConnection ? NetworkType.Unmetered : NetworkType.Any)
                        .Build();

                    var result = scheduler.Schedule(jobInfo);
                    if (result != JobScheduler.ResultSuccess) {
                        throw new Exception(string.Format("Failed to schedule job (schedule returned code {0})", result));
                    }

                    Log.Debug("Scheduled job with latency min {0} max {1} and required network {2}",
                        jobInfo.MinLatencyMillis,
                        jobInfo.MaxExecutionDelayMillis,
                        jobInfo.NetworkType);
                }
                else {
                    Log.Debug("Canceling job with id {0}", DataSyncJobId);
                    scheduler.Cancel(DataSyncJobId);
                }
            }
            catch (Exception ex) {
                Log.Error(ex, "Failed to setup synchronization on job scheduler");
            }
        }

    }

}
