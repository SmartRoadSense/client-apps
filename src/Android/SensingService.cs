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

        /// <summary>
        /// Sent when the service wakes up.
        /// </summary>
        public const string BroadcastIntentWakeUp = "it.uniurb.smartroadsense.sensing.wakeup";

        /// <summary>
        /// Sent when the service is torn down.
        /// </summary>
        public const string BroadcastIntentTearDown = "it.uniurb.smartroadsense.sensing.teardown";

        private const int NotificationRecordingId = 1;

        private static RecordingViewModel _model;

        /// <summary>
        /// Gets the recording view model, if any.
        /// </summary>
        public static RecordingViewModel ViewModel {
            get {
                return _model;
            }
        }

        private static ConcurrentQueue<Action<RecordingViewModel>> _actions = new ConcurrentQueue<Action<RecordingViewModel>>();

        public static void Do(Action<RecordingViewModel> action) {
            if (_model != null) {
                action(_model);
            }
            else {
                _actions.Enqueue(action);
            }
        }

        public override void OnCreate() {
            base.OnCreate();

            _model = new RecordingViewModel();
            _model.OnCreate();

            _model.RecordingStatusUpdated += HandleRecordingStatusUpdated;
        }

        private void HandleRecordingStatusUpdated(object sender, EventArgs e) {
            if (_model.IsRecording) {
                this.StartForeground(NotificationRecordingId, CreateRecordingNotification());
            }
            else {
                this.StopForeground(true);
            }
        }

        private Notification CreateRecordingNotification() {
            return new NotificationCompat.Builder(this)
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
            //Notify receivers that the service is up and running
            this.SendBroadcast(new Intent(BroadcastIntentWakeUp));

            //Perform queued actions
            while (_actions.Count > 0) {
                Action<RecordingViewModel> action;
                if (_actions.TryDequeue(out action)) {
                    action(_model);
                }
            }

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy() {
            base.OnDestroy();

            this.SendBroadcast(new Intent(BroadcastIntentTearDown));

            if (_model != null) {
                if (_model.IsRecording) {
                    _model.StopRecordingCommand.Execute(null);
                }

                _model.OnDestroy();
                _model = null;
            }
        }

        #region Implemented abstract members of Service

        public override IBinder OnBind(Intent intent) {
            return null;
        }

        #endregion

    }

}

