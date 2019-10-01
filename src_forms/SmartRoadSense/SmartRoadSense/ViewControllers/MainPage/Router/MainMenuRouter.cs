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

        public async Task<bool>  GoToRegisterPage()
        {
            return true;
        }

        public async Task<bool> GoToDataPage()
        {
            return true;
        }

        public async Task<bool> GoToStatsPage()
        {
            return true;
        }

        public async Task<bool> GoToSettingsPage()
        {
            await _view.CurrentPage.Navigation.PushAsync(new SettingsPage(_master), true);
            return true;
        }

        public async Task<bool> GoToInfoPage()
        {
            await _view.CurrentPage.Navigation.PushAsync(new InfoPage(_master), true);
            return true;
        }
    }
}
