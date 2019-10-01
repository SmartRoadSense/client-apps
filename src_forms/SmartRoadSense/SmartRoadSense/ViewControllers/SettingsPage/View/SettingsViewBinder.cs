using System;
namespace SmartRoadSense
{
    public class SettingsViewBinder : BaseViewBinder, ISettingsView
    {
        ISettingsInputActionPresenter _presenter;

        public SettingsViewBinder(SettingsPage page, MainMasterDetailPage master)
        {
            CurrentPage = page;
            var presenter = new SettingsPresenter(this, master);

            _presenter = presenter;
        }

        public SettingsPage CurrentPage { get; }

    }
}
