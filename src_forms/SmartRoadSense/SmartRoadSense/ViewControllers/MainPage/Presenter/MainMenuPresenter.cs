using System;
using System.Collections.ObjectModel;

namespace SmartRoadSense
{
    public class MainMenuPresenter : IMainMenuInputActionPresenter, IMainMenuOutputActionPresenter, IMainMenuDataPresenter
    {
        IMainMenuInteractor _interactor;
        IMainMenuView _view;
        IMainMenuRouter _router;

        public MainMenuPresenter(IMainMenuView view, MainMasterDetailPage master)
        {
            _view = view;
            _interactor = new MainMenuInteractor(this);
            _router = new MainMenuRouter(_view, master);
        }

        // DATA MODELS
        public ObservableCollection<MainMenuItem> MenuItems { get; set; }

        // ACTIONS
        public void ActionGoHome()
        {
            _router.GoToHomePage();
        }

        public void ActionLaunchService()
        {
            _interactor.LaunchService();
        }

        public void LaunchServiceActionSuccess(string message)
        {
            // TODO
        }

        public void LaunchServiceActionError(TrackDataException exception)
        {
            // TODO
        }
    }
}
