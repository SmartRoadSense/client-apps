using Xamarin.Essentials;

namespace SmartRoadSense {

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
            return new DeviceInformation
            {
                OperatingSystemName = DeviceInfo.Platform.ToString(),
                OperatingSystemVersion = DeviceInfo.VersionString,
                Manufacturer = DeviceInfo.Manufacturer,
                Model = DeviceInfo.Model
                // SdkVersion
            };
        }

        /// <summary>
        /// Gets a string representing the operating system type.
        /// </summary>
        public string OperatingSystemName { get; private set; }

        /// <summary>
        /// Gets a value representing the operating system version.
        /// </summary>
        public string OperatingSystemVersion { get; private set; }

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
