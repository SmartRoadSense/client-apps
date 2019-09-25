using Xamarin.Essentials;
using Xamarin.Forms;

namespace SmartRoadSense
{
    public class InfoViewBinder : BaseViewBinder, IInfoView
    {
        IInfoInputActionPresenter _presenter;
        IInfoDataPresenter _dataPresenter;

        public InfoViewBinder(InfoPage page, MainMasterDetailPage master)
        {
            CurrentPage = page;

            var presenter = new InfoPresenter(this, master);
            _presenter = presenter;
            _dataPresenter = presenter;
        }

        MainMasterDetailPage _master;

        public InfoPage CurrentPage { get; }

        // BINDINGS
        public string VersionLabel
        {
            get
            {
#if DEBUG
                return string.Format("{0} {1}", VersionTracking.CurrentVersion, AppResources.AboutVersionDebugLabel);
#elif BETA
                return string.Format("{0} {1}", VersionTracking.CurrentVersion, AppResources.AboutVersionBetaLabel);
#else
                return string.Format(VersionTracking.CurrentVersion);
#endif
            }
        }

        public string AppInfoLabel
        {
            get => string.Format("{0} v{1} on {2} {3}, running on {4} {5}", AppInfo.Name, AppInfo.VersionString, DeviceInfo.Platform, DeviceInfo.VersionString, DeviceInfo.Manufacturer, DeviceInfo.Model);  // Device.RuntimePlatform, Device.Idiom);
        }

        // ACTIONS
    }
}
