using System;
using System.Linq;

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

            // TODO: get data from settings
            // Init current vehicle type
            currentVehicleType = VehicleSource.CAR;
            currentAnchorageType = AnchorageSource.MOUNT;
        }

        public SettingsPage CurrentPage { get; }

        public VehicleSource currentVehicleType;
        public AnchorageSource currentAnchorageType;

        public string VehicleTextSource
        {
            get => SettingsViewSource.VehicleTypeStrings[currentVehicleType];
            set
            {
                currentVehicleType = SettingsViewSource.VehicleTypeStrings.Keys.First(v => SettingsViewSource.VehicleTypeStrings[v].Equals(value));
                OnPropertyChanged();
            }
        }

        public string VehicleBtnSource
        {
            get => SettingsViewSource.VehicleIconSource[currentVehicleType];
            set
            {
                currentVehicleType = SettingsViewSource.VehicleIconSource.Keys.First(v => SettingsViewSource.VehicleIconSource[v].Equals(value));
                OnPropertyChanged();
            }
        }

        public string AnchorageTextSource
        {
            get => SettingsViewSource.AnchorageTypeStrings[currentAnchorageType];
            set
            {
                currentAnchorageType = SettingsViewSource.AnchorageTypeStrings.Keys.First(v => SettingsViewSource.AnchorageTypeStrings[v].Equals(value));
                OnPropertyChanged();
            }
        }

        public string AnchorageBtnSource
        {
            get => SettingsViewSource.AnchorageIconSource[currentAnchorageType];
            set
            {
                currentAnchorageType = SettingsViewSource.AnchorageIconSource.Keys.First(v => SettingsViewSource.AnchorageIconSource[v].Equals(value));
                OnPropertyChanged();
            }
        }
    }
}
