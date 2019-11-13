using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SmartRoadSense.Core;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Diagnostics;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SmartRoadSense
{
    public partial class App : Application
    {
        private static bool _initialized;

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

        public App()
        {
            VersionTracking.Track();
            InitializeComponent();
            InitializeTheme();
            InitializeSrsEngine();

            MainPage = new MainMasterDetailPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            InitializeLocalization();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            InitializeLocalization();
        }

        async void InitializeSrsEngine()
        {
            Log.Debug("Initializing synchronous tasks");

            RegisterAnalytics();

            Engine = new Engine();
            Sensors = SensorPack.Create(Engine);
            Recorder = new Recorder(Engine);
            Sync = new SyncManager();

            await InitializeEngine();
        }

        async Task InitializeEngine()
        {
            Log.Debug("Initializing asynchronous tasks");

            await UserLog.Initialize().ConfigureAwait(false);
            UserLog.Add(string.Format(AppResources.Started, Version));

            await FileNaming.InitializeFileStructure().ConfigureAwait(false);

            await DatabaseUtility.Initialize();

            _initialized = true;
        }

        void InitializeTheme()
        {
            var ColorInfo = new ColorUtility();
            Resources["ThemePrimaryColor"] = ColorInfo.ThemePrimaryColor;
            Resources["ThemeSecondaryColor"] = ColorInfo.ThemeSecondaryColor;
            Resources["ThemeTerziaryColor"] = ColorInfo.ThemeTerziaryColor;
            Resources["ThemePrimaryDarkColor"] = ColorInfo.ThemePrimaryDarkColor;
            Resources["ThemePrimaryDarkLightenedColor"] = ColorInfo.ThemePrimaryDarkLightenedColor;
            Resources["ThemePrimaryPressedColor"] = ColorInfo.ThemePrimaryPressedColor;
            Resources["DrawerBackgroundColor"] = ColorInfo.DrawerBackgroundColor;
            Resources["DefaultBackgroundColor"] = ColorInfo.DefaultBackgroundColor;
            Resources["TextOnDarkColor"] = ColorInfo.TextOnDarkColor;
            Resources["SubtleTextOnDarkColor"] = ColorInfo.SubtleTextOnDarkColor;
            Resources["SubduedTextOnDarkColor"] = ColorInfo.SubduedTextOnDarkColor;
            Resources["TextOnBrightColor"] = ColorInfo.TextOnBrightColor;
            Resources["SubtleTextOnBrightColor"] = ColorInfo.SubtleTextOnBrightColor;
            Resources["LightGrayColor"] = ColorInfo.LightGrayColor;
            Resources["LightGrayPressedColor"] = ColorInfo.LightGrayPressedColor;
            Resources["TextOnLightGrayColor"] = ColorInfo.TextOnLightGrayColor;
            Resources["QualityGoodColor"] = ColorInfo.QualityGoodColor;
            Resources["QualityBadColor"] = ColorInfo.QualityBadColor;
            Resources["ErrorColor"] = ColorInfo.ErrorColor;
        }

        void InitializeLocalization()
        {
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                AppResources.Culture = ci; // set the RESX for resource localization
                DependencyService.Get<ILocalize>().SetLocale(ci); // set the Thread for locale-aware methods
            }
        }

        #region Lifetime

        [Conditional("DEBUG")]
        private static void VerifyInitialization()
        {
            if (!_initialized)
                throw new InvalidOperationException("App instance not initialized");
        }

        /// <summary>
        /// Should be called on application activation (usually a user-initiated
        /// launch which shows some UI).
        /// </summary>
        public static void Activated()
        {
            VerifyInitialization();

            Log.Debug("Application activated");
        }

        /// <summary>
        /// Should be called when the application is suspended or is backgrounded.
        /// </summary>
        /// <remarks>
        /// Can be awaited on synchronously (does not capture context).
        /// </remarks>
        public static async Task Suspended()
        {
            VerifyInitialization();

            Log.Debug("Application suspended");

            await UserLog.Persist().ConfigureAwait(false);
        }

        #endregion Lifetime

        /// <summary>
        /// Gets a description of the application and device information.
        /// </summary>
        public static string ApplicationInformation
        {
            get
            {
                var deviceInfo = DeviceInformation.Current;

                return string.Format(
#if DEBUG
                    AppResources.AboutVersionDebugLabel,
#elif BETA
                    AppResources.AboutVersionBetaLabel,
#else
                    AppResources.SmartRoadSenseLabel
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
        public static JsonSerializerSettings JsonSettings
        {
            get
            {
                var settings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    NullValueHandling = NullValueHandling.Ignore
                };

                return settings;
            }
        }

        private static void RegisterAnalytics()
        {
            //#if !DEBUG && !DESKTOP
            Microsoft.AppCenter.AppCenter.Start
            (
                "ios=" + GetConfigKey("AppCenterApiSecretIos") + ";android=" + GetConfigKey("AppCenterApiSecretDroid"),
                typeof(Microsoft.AppCenter.Analytics.Analytics),
                typeof(Microsoft.AppCenter.Crashes.Crashes)
            );
            //#endif
        }

        #region Configuration

        private const string ConfigurationFilenameRoot = "Config.json";

        private static Dictionary<string, string> _configMap;

        private static void LoadConfig()
        {
            if (_configMap != null)
                return;

            _configMap = new Dictionary<string, string>();

            try
            {
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(App));
                string[] resourceNames = assembly.GetManifestResourceNames();
                var fullname = (from r in resourceNames where r.EndsWith(ConfigurationFilenameRoot, StringComparison.CurrentCulture) select r).FirstOrDefault();

                using (var s = assembly.GetManifestResourceStream(fullname))
                {
                    using (var reader = new System.IO.StreamReader(s))
                    {
                        var txt = reader.ReadToEnd();
                        _configMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(txt);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Err: " + e);
                // Ignore this for now
            }
        }

        public static string GetConfigKey(string key)
        {
            LoadConfig();

            if (_configMap.ContainsKey(key))
            {
                return _configMap[key];
            }
            return null;
        }

        #endregion

        private static Version _version = null;

        /// <summary>
        /// Gets the application version.
        /// </summary>
        public static Version Version
        {
            get
            {
                if (_version == null)
                {
                    _version = InitVersion();
                }

                return _version;
            }
        }

        private static Version InitVersion()
        {
            return new Version(VersionTracking.CurrentVersion);
        }
    }
}
