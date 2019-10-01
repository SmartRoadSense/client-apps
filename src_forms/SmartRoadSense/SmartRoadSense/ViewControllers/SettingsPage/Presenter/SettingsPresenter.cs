using System;
namespace SmartRoadSense
{
    public class SettingsPresenter : ISettingsInputActionPresenter, ISettingsOutputActionPresenter
    {
        ISettingsView _view;
        ISettingsRouter _router;
        ISettingsInteractor _interactor;

        public SettingsPresenter(ISettingsView view, MainMasterDetailPage master)
        {
            _view = view;
            _router = new SettingsRouter(view, master);
            _interactor = new SettingsInteractor(this);
        }
    }
}
