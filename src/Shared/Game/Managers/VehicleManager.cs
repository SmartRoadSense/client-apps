using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared
{
    // Vehicle manager - Type, upgrades, etc.
    class VehicleManager : IVehicleManager, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static VehicleManager Instance { get; } = new VehicleManager();

        public int VehicleCount {
            get => Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.VehicleCount.Value, 0);
            set {
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.VehicleCount.Value, value);
                OnPropertyChanged();
            }
        }

        public int CurrentGarageVehicleId
        {
            get => Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.SelectedGarageVehicle.Value, -1);
            set {
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.SelectedGarageVehicle.Value, value);
                OnPropertyChanged();
            }
        }

        public int SelectedVehicleId
        {
            get => Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.IdVehicle.Value, -1);
            set 
            {
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.IdVehicle.Value, value);
                OnPropertyChanged();
            }
        }

        public VehicleModel SelectedVehicleModel
        {
            get
            {
                var json = Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.SelectedVehicle.Value, "");
                return string.IsNullOrEmpty(json)? null : JsonConvert.DeserializeObject<VehicleModel>(json);
            }
            set 
            {
                var json = JsonConvert.SerializeObject(value);
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.SelectedVehicle.Value, json);
                OnPropertyChanged();
            }
        }

        public int VehiclesUnlocked 
        {
            get 
            {
                var unlocked = 0;
                var vehicles = JsonReaderVehicles.GetVehicles();
                foreach(var vehicle in vehicles.VehicleModel) {
                    if(vehicle.UnlockCoins <= CharacterManager.Instance.User.Level)
                        unlocked += 1;
                }
                return unlocked;
            }
        }
    }
}
