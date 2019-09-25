using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SmartRoadSense
{
    public partial class App : Application
    {
        public App()
        {
            VersionTracking.Track();

            MainPage = new MainMasterDetailPage();

            InitializeComponent();
            InitializeTheme();
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
    }
}
