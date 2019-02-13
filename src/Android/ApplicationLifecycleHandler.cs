using System;
using Android.App;
using Android.OS;
using SmartRoadSense.Shared;
using System.Threading;

namespace SmartRoadSense.Android {

    public class ApplicationLifecycleHandler : Java.Lang.Object, Application.IActivityLifecycleCallbacks {

        private static int _referenceCount = 0;

        private static readonly string Tag = typeof(ApplicationLifecycleHandler).FullName;

        #region IActivityLifecycleCallbacks implementation

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState) {

        }

        public void OnActivityDestroyed(Activity activity) {

        }

        public void OnActivityPaused(Activity activity) {

        }

        public void OnActivityResumed(Activity activity) {

        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState) {

        }

        public void OnActivityStarted(Activity activity) {
            if(_referenceCount == 0) {
                Log.Debug("Application UI resumed");

                App.Activated();
            }

            Interlocked.Increment(ref _referenceCount);

            Log.Debug("Activity started (count {0})", _referenceCount);
        }

        public void OnActivityStopped(Activity activity) {
            Interlocked.Decrement(ref _referenceCount);

            Log.Debug("Activity stopped (count {0})", _referenceCount);

            if(_referenceCount == 0) {
                App.Suspended().Wait();

                Log.Debug("Application UI suspended");
            }
        }

        #endregion

    }

}
