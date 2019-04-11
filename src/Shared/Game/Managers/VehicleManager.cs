using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using System.Diagnostics;

namespace SmartRoadSense.Shared
{
    // Vehicle manager - Type, upgrades, etc.
    class VehicleManager : IVehicleManager, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static VehicleManager Instance { get; } = new VehicleManager();

        public void Init() {
            List<CollectedComponents> components = new List<CollectedComponents>();
            if(CollectedComponents != null && CollectedComponents.CollectedComponentsList != null)
                components = CollectedComponents.CollectedComponentsList;

            foreach(var v in Vehicles.VehicleModel) {
                if(v.UnlockCost < 0 && !components.Exists(e => e.VehicleId == v.IdVehicle)) {
                    components.Add(new CollectedComponents { VehicleId = v.IdVehicle, VehicleComponents = new Components() });
                }
            }

            if(CollectedComponents == null) {
                CollectedComponents = new CollectedComponentsContainer {
                    CollectedComponentsList = components
                };
            }

            else {
                var collected = CollectedComponents;
                collected.CollectedComponentsList = components;
                CollectedComponents = collected;
            }
        }

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

        public VehicleContainerModel VehiclesUnlocked 
        {
            get {
                var json = Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.UnlockedVehicles.Value, "");
                var vehicleIdList = string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<VehiclesUnlockedList>(json);
                if(vehicleIdList == null || vehicleIdList.VehicleIds.IsEmpty())
                    return null;

                var vehicles = new VehicleContainerModel {
                    VehicleModel = new List<VehicleModel>()
                };

                foreach(var id in vehicleIdList.VehicleIds) {
                    vehicles.VehicleModel.Add(JsonReaderVehicles.GetSingleVehicle(id));
                }
                return vehicles;
            }
            set {
                var json = JsonConvert.SerializeObject(value);
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.UnlockedVehicles.Value, json);
                OnPropertyChanged();
            }
        }
        public VehicleContainerModel Vehicles 
        {
            get => JsonReaderVehicles.GetVehicles();
        }

        public CollectedComponentsContainer CollectedComponents
        {
            get {
                var json = Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.CollectedComponents.Value, "");
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<CollectedComponentsContainer>(json);
            }
            set {
                var json = JsonConvert.SerializeObject(value);
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.CollectedComponents.Value, json);
                OnPropertyChanged();
            }
        }
    }
}
