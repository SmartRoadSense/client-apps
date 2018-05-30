using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SmartRoadSense.Core;
using SmartRoadSense.Resources;
using SmartRoadSense.Shared.Data;
using SmartRoadSense.Shared.Database;
using Newtonsoft.Json;

#if __ANDROID__
using Android.App;
using Android.Content;
using Android.Content.PM;
#elif __IOS__
using Foundation;
using ObjCRuntime;
#elif WINDOWS_PHONE_APP
using Windows.ApplicationModel;
using Windows.Phone.Management.Deployment;
using SmartRoadSense.WindowsPhone81;
#endif

namespace SmartRoadSense.Shared {

    public static partial class App {

        private static bool _initialized = false;

#if __ANDROID__

        /// <summary>
        /// Initializes the application.
        /// </summary>
        /// <remarks>
        /// Can be awaited on synchronously (does not capture context).
        /// </remarks>
        public static async Task Initialize(Application androidApp, Context context) {
            AndroidApplication = androidApp;
            Context = context;

#elif __IOS__

        /// <summary>
        /// Initializes the application.
        /// </summary>
        /// <remarks>
        /// Can be awaited on synchronously (does not capture context).
        /// </remarks>
        public static async Task Initialize() {

#elif WINDOWS_PHONE_APP

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public static async Task Initialize() {

#else

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public static async Task Initialize() {
            throw new InvalidOperationException();

#endif

            Log.Debug("Initializing {0}", ApplicationInformation);

            RegisterAnalytics();

            Engine = new Engine();
            Sensors = SensorPack.Create(Engine);
            Recorder = new Recorder(Engine);
            Sync = new SyncManager();

            await UserLog.Initialize().ConfigureAwait(false);
            UserLog.Add(string.Format(LogStrings.Started, Version));

            await FileNaming.InitializeFileStructure().ConfigureAwait(false);

            await DatabaseUtility.Initialize();

            _initialized = true;
        }

        #region Lifetime

        [Conditional("DEBUG")]
        private static void VerifyInitialization() {
            if (!_initialized)
                throw new InvalidOperationException("App instance not initialized");
        }

        /// <summary>
        /// Should be called on application activation (usually a user-initiated
        /// launch which shows some UI).
        /// </summary>
        public static void Activated() {
            VerifyInitialization();

            Log.Debug("Application activated");
        }

        /// <summary>
        /// Should be called when the application is suspended or is backgrounded.
        /// </summary>
        /// <remarks>
        /// Can be awaited on synchronously (does not capture context).
        /// </remarks>
        public static async Task Suspended() {
            VerifyInitialization();

            Log.Debug("Application suspended");

            await UserLog.Persist().ConfigureAwait(false);
        }

        #endregion Lifetime

        #region App level components

        /// <summary>
        /// Gets the app's core algorithmic engine.
        /// </summary>
        public static Engine Engine { get; private set; }

        /// <summary>
        /// Gets the app's sensor pack.
        /// </summary>
        public static SensorPack Sensors { get; private set; }

        /// <summary>
        /// Gets the app's data recorder.
        /// </summary>
        public static Recorder Recorder { get; private set; }

        /// <summary>
        /// Gets the app's sync manager.
        /// </summary>
        public static SyncManager Sync { get; private set; }

        #endregion App level components

        private static Version _version = null;

        /// <summary>
        /// Gets the application version.
        /// </summary>
        public static Version Version {
            get {
                if (_version == null) {
                    _version = InitVersion();
                }

                return _version;
            }
        }

        private static Version InitVersion() {
#if __ANDROID__
            PackageInfo package;
            try {
                package = Context.PackageManager.GetPackageInfo(Context.PackageName, (PackageInfoFlags)0);
            }
            catch (PackageManager.NameNotFoundException) {
                return new Version(1, 0);
            }

            Version ret;
            if (Version.TryParse(package.VersionName, out ret))
                return ret;
            else
                return new Version(1, 0);
#elif __IOS__
            return new Version(NSBundle.MainBundle.InfoDictionary [new NSString ("CFBundleShortVersionString")].ToString ());
#elif WINDOWS_PHONE_APP
            var packages = InstallationManager.FindPackagesForCurrentPublisher();
            var package = packages.FirstOrDefault();

            if (package != null)
                return package.Id.Version.ToVersion();
            else
                return new Version(1, 0);
#elif DESKTOP
            return System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
#else
#error Unrecognized platform
#endif
        }

        /// <summary>
        /// Gets a description of the application and device information.
        /// </summary>
        public static string ApplicationInformation {
            get {
                var deviceInfo = DeviceInformation.Current;

                return string.Format(
#if DEBUG
                    AppStrings.DebugApplicationDescriptionFormat,
#elif BETA
                    AppStrings.BetaApplicationDescriptionFormat,
#else
                    AppStrings.ApplicationDescriptionFormat,
#endif
                    Version,
                    deviceInfo.OperatingSystemName,
                    deviceInfo.OperatingSystemVersion,
                    deviceInfo.SdkVersion,
                    deviceInfo.Manufacturer,
                    deviceInfo.Model
                );
            }
        }

        /// <summary>
        /// Gets app-wide JSON serialization settings.
        /// </summary>
        public static JsonSerializerSettings JsonSettings {
            get {
                var settings = new JsonSerializerSettings();
                settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

                return settings;
            }
        }

        #region Configuration

        private const string ConfigurationFilenameRoot = "Config.json";

        private static Dictionary<string, string> _configMap = null;

        private static void LoadConfig() {
            if(_configMap != null)
                return;

            _configMap = new Dictionary<string, string>();

            try {
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(App));
                string[] resourceNames = assembly.GetManifestResourceNames();
                var fullname = (from r in resourceNames where r.EndsWith(ConfigurationFilenameRoot) select r).FirstOrDefault();

                using(var s = assembly.GetManifestResourceStream(fullname)) {
                    using(var reader = new System.IO.StreamReader(s)) {
                        var txt = reader.ReadToEnd();
                        _configMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(txt);
                    }
                }
            }
            catch(Exception) {
                // Ignore this for now
            }
        }

        public static string GetConfigKey(string key) {
            LoadConfig();

            if(_configMap.ContainsKey(key)) {
                return _configMap[key];
            }
            else {
                return null;
            }
        }

        #endregion

        static void RegisterAnalytics() {
#if !DEBUG && !DESKTOP
            Microsoft.AppCenter.AppCenter.Start(
                GetConfigKey("AppCenterApiSecret"),
                typeof(Microsoft.AppCenter.Analytics.Analytics),
                typeof(Microsoft.AppCenter.Crashes.Crashes)
            );
#endif
        }

    }

}
