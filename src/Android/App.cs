using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SmartRoadSense.Shared {

    public static partial class App {

        public static Context Context { get; private set; }

        public static Application AndroidApplication { get; private set; }

        private static Version InitPlatformVersion() {
            PackageInfo package;
            try {
                package = Context.PackageManager.GetPackageInfo(Context.PackageName, 0);
            }
            catch (PackageManager.NameNotFoundException) {
                return new Version(1, 0);
            }

            if (Version.TryParse(package.VersionName, out Version ret))
                return ret;
            else
                return new Version(1, 0);
        }

    }

}
