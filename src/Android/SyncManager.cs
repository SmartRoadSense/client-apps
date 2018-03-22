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

using SmartRoadSense.Android;

namespace SmartRoadSense.Shared {

    public partial class SyncManager {

        private bool CheckPlatformSyncConditions() {
            if(!App.Context.CheckConnection()) {
                Log.Debug("Can't sync: no available connection");
                return false;
            }

            return true;
        }

    }

}