using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;

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
            // Update components list
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

            // TODO: update unlocked vehicles list
            var vehiclesOrig = UnlockedVehicles;
            foreach(var v in vehiclesOrig.VehicleModel) {
                var vehicleData = Vehicles.VehicleModel.First(m => m.IdVehicle == v.IdVehicle);
                vehiclesOrig.VehicleModel.First(m => m.IdVehicle == v.IdVehicle).UpdateVehicleModel(vehicleData);
            }
            UnlockedVehicles = vehiclesOrig;
        }

        public int VehicleCount {
            get => Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.VehicleCount.Value, 0);
            set {
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.VehicleCount.Value, value);
                OnPropertyChanged();
            }
        }

        public VehicleModel SelectedVehicleModel {
            get {
                var json = Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.SelectedVehicle.Value, "");
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<VehicleModel>(json);
            }
            set {
                var json = JsonConvert.SerializeObject(value);
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.SelectedVehicle.Value, json);
                OnPropertyChanged();
            }
        }

        public VehicleModel GetVehicleFromId(int id) {
            return Vehicles.VehicleModel.Find(v => v.IdVehicle == id);
        }

        public VehicleContainerModel UnlockedVehicles 
        {
            get {
                var json = Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.UnlockedVehicles.Value, "");
                var vehicleIdList = string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<VehicleContainerModel>(json);
                if(vehicleIdList == null || vehicleIdList.VehicleModel.IsEmpty())
                    return new VehicleContainerModel();

                return vehicleIdList;
            }
            set {
                var json = JsonConvert.SerializeObject(value);
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.UnlockedVehicles.Value, json);
                OnPropertyChanged();
            }
        }

        public void UnlockVehicle() {
            var vehiclesUnlocked = Instance.UnlockedVehicles;
            if(!vehiclesUnlocked.VehicleModel.Contains(Instance.SelectedVehicleModel)) {
                vehiclesUnlocked.VehicleModel.Add(Instance.SelectedVehicleModel);
                Instance.UnlockedVehicles = vehiclesUnlocked;
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

        public CollectedComponents CollectedComponentsForVehicle(int vehicleId) {
            return Instance.CollectedComponents.CollectedComponentsList.FirstOrDefault(v => v.VehicleId == vehicleId);
        }
    }
}
