using System;
using Android.App;
using Android.Runtime;

using SmartRoadSense.Shared;

namespace SmartRoadSense.Android {

    [Application]
    public class SmartRoadSenseApplication : Application {

        private readonly ApplicationLifecycleHandler _lifecycleHandler = new ApplicationLifecycleHandler();

        public SmartRoadSenseApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) {
        }

        public override void OnCreate() {
            base.OnCreate();

            Log.Debug("Synchronous app initialization");

            App.Initialize(this, ApplicationContext).Wait();

            Log.Debug("Completing Android initialization");

            RegisterActivityLifecycleCallbacks(_lifecycleHandler);

            DataSyncReceiver.AdaptiveConfigureSync(this).Forget();
            App.Recorder.DataFileWritten += (sender, e) => {
                App.Sync.Synchronize(System.Threading.CancellationToken.None, SyncPolicy.ForceLast).Forget();
            };

            CalibrationUi = new CalibrationUiManager();
        }

        public static CalibrationUiManager CalibrationUi { get; private set; }

    }

}
