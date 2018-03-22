using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SmartRoadSense.Shared;

namespace SmartRoadSense.Android {

    public static class ContextExtensions {

        /// <summary>
        /// Checks whether there is a valid connection available and whether it satisfies
        /// the connection requirements given by the user (i.e., Wi-Fi only).
        /// </summary>
        /// <returns>
        /// True if the connection can be used.
        /// </returns>
        public static bool CheckConnection(this Context context) {
            var connectivity = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);

            var activeNetwork = connectivity.ActiveNetworkInfo;
            if (activeNetwork == null) {
                Log.Debug("No active network");
                return false;
            }

            if (!activeNetwork.IsConnectedOrConnecting) {
                Log.Debug("Active network is not connected");
                return false;
            }

            if (!Settings.PreferUnmeteredConnection) {
                //We have a connection and the user doesn't care about metering
                Log.Debug("Active network available, don't care if unmetered");
                return true;
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean) {
                Log.Debug("Active network available, is metered: {0}", connectivity.IsActiveNetworkMetered);
                return !connectivity.IsActiveNetworkMetered;
            }

            Log.Debug("Active network available, type: {0}, is considered metered: {1}",
                activeNetwork.Type, IsNetworkTypeMetered(activeNetwork.Type));

            return IsNetworkTypeMetered(activeNetwork.Type);
        }

        /// <summary>
        /// Determines whether a connectivity type is usually considered to be metered or not.
        /// </summary>
        /// <remarks>
        /// Wi-Fi and ethernet are considered unmetered.
        /// </remarks>
        private static bool IsNetworkTypeMetered(ConnectivityType connectivityType) {
            return (
                connectivityType == ConnectivityType.Wifi ||
                connectivityType == ConnectivityType.Ethernet
            );
        }

    }

}