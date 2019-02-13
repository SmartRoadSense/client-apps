using System;

namespace SmartRoadSense.Shared {

    public static partial class App {

        private static Version InitPlatformVersion() {
            return System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
        }

    }

}
