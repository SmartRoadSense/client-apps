using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartRoadSense
{
    public class SettingsManager : ISettingsManager, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        static readonly Lazy<SettingsManager> _instance = new Lazy<SettingsManager>(() => new SettingsManager());

        public SettingsManager Instance => _instance.Value;

        public AnchorageType CurrentAnchorageType { get; set; }

        public VehicleType CurrentVehicleType { get; set; }
    }
}
