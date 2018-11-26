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
        }
    }
}
