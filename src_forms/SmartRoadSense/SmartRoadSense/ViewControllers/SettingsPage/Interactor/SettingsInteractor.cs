using System;
namespace SmartRoadSense
{
    public class SettingsInteractor : ISettingsInteractor
    {
        ISettingsOutputActionPresenter _presenter;

        public SettingsInteractor(ISettingsOutputActionPresenter presenter)
        {
            _presenter = presenter;
        }
    }
}
