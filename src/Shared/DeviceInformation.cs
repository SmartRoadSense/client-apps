using System;
using System.Collections.Generic;
using System.Text;

using SmartRoadSense.Resources;

#if __IOS__
using ObjCRuntime;
using UIKit;
#elif WINDOWS_PHONE_APP
using Windows.Security.ExchangeActiveSyncProvisioning;
#endif

namespace SmartRoadSense.Shared {

    /// <summary>
    /// Contains information about the current device on which the app is running.
    /// </summary>
    public class DeviceInformation {

        //Prevent instantiation
        private DeviceInformation() {
        }

        private static DeviceInformation _currentInformation = null;

        /// <summary>
        /// Gets information about the device.
        /// </summary>
        public static DeviceInformation Current {
            get {
                if (_currentInformation == null) {
                    _currentInformation = Generate();
                }

                return _currentInformation;
            }
        }

        private static DeviceInformation Generate() {
#if __ANDROID__
            return new DeviceInformation {
                OperatingSystemName = "Android",
                OperatingSystemVersion = new Version(global::Android.OS.Build.VERSION.Release),
                SdkVersion = string.Format(AppStrings.SdkVersionFormat, global::Android.OS.Build.VERSION.SdkInt.ToString()),
                Manufacturer = global::Android.OS.Build.Manufacturer.ToTitleCase(),
                Model = global::Android.OS.Build.Model.ToTitleCase()
            };
#elif __IOS__
            return new DeviceInformation {
                OperatingSystemName = "iOS",
                OperatingSystemVersion =  new Version(UIDevice.CurrentDevice.SystemVersion),
                Manufacturer = "Apple",
                Model = UIDevice.CurrentDevice.Model
            };
#elif WINDOWS_PHONE_APP
            var eas = new EasClientDeviceInformation();

            //eas.OperatingSystem; //TODO?

            return new DeviceInformation {
                OperatingSystemName = "Windows Phone",
                OperatingSystemVersion = new Version(8, 1),
                Manufacturer = eas.SystemManufacturer,
                Model = eas.SystemProductName
            };
#elif DESKTOP
            return new DeviceInformation {
                OperatingSystemName = Environment.OSVersion.Platform.ToString(),
                OperatingSystemVersion = Environment.OSVersion.Version,
                Manufacturer = "Unknown",
                Model = "Unknown"
            };
#else
#error Unrecognized platform
#endif
        }

        /// <summary>
        /// Gets a string representing the operating system type.
        /// </summary>
        public string OperatingSystemName { get; private set; }

        /// <summary>
        /// Gets a value representing the operating system version.
        /// </summary>
        public Version OperatingSystemVersion { get; private set; }

        /// <summary>
        /// Gets a string representing the SDK version that generated this application.
        /// </summary>
        public string SdkVersion { get; private set; }

        /// <summary>
        /// Gets the manifacturer name of the device.
        /// </summary>
        public string Manufacturer { get; private set; }

        /// <summary>
        /// Gets the model name of the device.
        /// </summary>
        public string Model { get; private set; }

    }

}
