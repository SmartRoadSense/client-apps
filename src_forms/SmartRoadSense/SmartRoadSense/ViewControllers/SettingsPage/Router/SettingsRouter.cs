using System;
namespace SmartRoadSense
{
    public class SettingsRouter : ISettingsRouter
    {
        MainMasterDetailPage _master;
        ISettingsView _view;

        public SettingsRouter(ISettingsView view, MainMasterDetailPage master)
        {
            _master = master;
            _view = view;
        }
    }
}
