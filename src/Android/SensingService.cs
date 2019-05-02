using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

using SmartRoadSense.Core;
using SmartRoadSense.Shared;
using SmartRoadSense.Shared.Data;
using SmartRoadSense.Shared.ViewModel;

namespace SmartRoadSense.Android {

    [Service(
        Label = "@string/Vernacular_P0_service_sensing"
    )]
    public class SensingService : Service {

        private const int NotificationRecordingId = 1;

        private static RecordingViewModel _model = null;

        /// <summary>
        /// Gets the recording view model, if any.
        /// </summary>
        public static RecordingViewModel ViewModel {
            get {
                if(_model == null) {
                    _model = new RecordingViewModel();
                    _model.OnCreate();
                }

                return _model;
            }
        }

        public static void Do(Action<RecordingViewModel> action) {
            action(ViewModel);
        }

        public override void OnCreate() {
            base.OnCreate();

            ViewModel.RecordingStatusUpdated += HandleRecordingStatusUpdated;
        }

        private void HandleRecordingStatusUpdated(object sender, EventArgs e) {
            if (_model.IsRecording) {
                StartForeground(NotificationRecordingId, CreateRecordingNotification());
            }
            else {
                StopForeground(true);
            }
        }

        public const string NotificationChannelId = "it.uniurb.smartroadsense.recording";
        public const string NotificationChannelDescription = "SmartRoadSense sensing";

        private Notification CreateRecordingNotification() {
            string notificationChannel = NotificationChannel.DefaultChannelId;
            if(Build.VERSION.SdkInt >= BuildVersionCodes.O) {
                var ch = new NotificationChannel(NotificationChannelId, NotificationChannelDescription, NotificationImportance.Default) {
                    LockscreenVisibility = NotificationVisibility.Public,
                };

                var notman = (NotificationManager)GetSystemService(Context.NotificationService);
                notman.CreateNotificationChannel(ch);

                notificationChannel = NotificationChannelId;
            }

            return new NotificationCompat.Builder(this, notificationChannel)
                .SetContentTitle(GetString(Resource.String.Vernacular_P0_sensing_service_notification_title))
                .SetContentText(GetString(Resource.String.Vernacular_P0_sensing_service_notification_message))
                .SetSmallIcon(Resource.Drawable.ic_status)
                .SetOngoing(true)
                .SetWhen(App.Recorder.Session.StartTimestampUtc.ToUnixEpochMilliseconds())
                .SetUsesChronometer(true)
                .SetVisibility(NotificationCompat.VisibilityPublic)
                .SetContentIntent(
                    PendingIntent.GetActivity(ApplicationContext, 0,
                        new Intent(this, typeof(MainActivity)), PendingIntentFlags.UpdateCurrent))
                .Build();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId) {
            ViewModel.OnCreate();

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy() {
            base.OnDestroy();

            if(ViewModel.IsRecording) {
                ViewModel.StopRecordingCommand.Execute(null);
            }
            ViewModel.RecordingStatusUpdated -= HandleRecordingStatusUpdated;
        }

        #region Implemented abstract members of Service

        public override IBinder OnBind(Intent intent) {
            return null;
        }

        #endregion

    }

}

