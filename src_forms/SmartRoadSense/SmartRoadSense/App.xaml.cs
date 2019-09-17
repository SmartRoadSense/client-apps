using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SmartRoadSense
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            InitializeTheme();

            MainPage = new MainMasterDetailPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
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
    }
}
