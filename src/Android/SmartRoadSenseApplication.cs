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

            try {
                App.Initialize(this, ApplicationContext).Wait();
            }
            catch(Exception ex) {
                Log.Error(ex, "Failed to initialize app");
                throw;
            }

            Log.Debug("Completing Android initialization");

            RegisterActivityLifecycleCallbacks(_lifecycleHandler);

            DataSyncReceiver.ConfigureSync(this);

            CalibrationUi = new CalibrationUiManager();
        }

        public static CalibrationUiManager CalibrationUi { get; private set; }

    }

}
