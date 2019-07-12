using System;
using System.Threading.Tasks;

namespace SmartRoadSense
{
    public class MainMenuRouter : IMainMenuRouter
    {
        IMainMenuView _view;
        MainMasterDetailPage _master;

        public MainMenuRouter(IMainMenuView view, MainMasterDetailPage master)
        {
            _master = master;
            _view = view;
        }

        public async Task<bool> GoToHomePage()
        {
            // TODO
            await _view.CurrentPage.Navigation.PushAsync(new MainPageMenu(_master), true);
            return true;
        }

        public async void GoToRegisterPage()
        {
            // TODO
        }

        public async void GoToDataPage()
        {
            // TODO
        }

        public async void GoToStatsPage()
        {
            // TODO
        }

        public async void GoToSettingsPage()
        {
            // TODO
        }

        public async void GoToInfoPage()
        {
            // TODO
        }
    }
}
