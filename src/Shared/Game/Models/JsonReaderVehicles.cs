using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared {
    public static class JsonReaderVehicles {
        const string VehicleJsonFilename = "Vehicles.json";

        static VehicleContainerModel vehicleContainer = null;

        static void LoadConfig() 
        {
            if(vehicleContainer != null)
                return;
               
            try {
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(App));
                string[] resourceNames = assembly.GetManifestResourceNames();
                var fullname = (from r in resourceNames where r.EndsWith(VehicleJsonFilename, StringComparison.Ordinal) select r).FirstOrDefault();

                using(var s = assembly.GetManifestResourceStream(fullname)) {
                    using(var reader = new System.IO.StreamReader(s)) {
                        var txt = reader.ReadToEnd();
                        JsonSerializer serializer = new JsonSerializer();
                        vehicleContainer = JsonConvert.DeserializeObject<VehicleContainerModel>(txt);
                    }
                }
            }
            catch(Exception e) {
                System.Diagnostics.Debug.WriteLine("Error decoding vehicle file: " + e);
            }
        }

        public static void GetVehicleConfig() 
        {
            LoadConfig();
            if(vehicleContainer != null) 
            {
                System.Diagnostics.Debug.WriteLine("vehicle count: {0}", vehicleContainer.VehicleModel.Count);
                VehicleManager.Instance.VehicleCount = vehicleContainer.VehicleModel.Count;
            }
        }

        public static void GetSingleVehicle(int id) 
        {
            LoadConfig();
            var vehicle = vehicleContainer.VehicleModel.FirstOrDefault(vehicleContainer => vehicleContainer.IdVehicle == id);
            VehicleManager.Instance.SelectedVehicleModel = vehicle;   
                
            System.Diagnostics.Debug.WriteLine("name: " + vehicle.Name);
            System.Diagnostics.Debug.WriteLine("brake: " + vehicle.Brake);
            System.Diagnostics.Debug.WriteLine("id: " + vehicle.IdVehicle);
            System.Diagnostics.Debug.WriteLine("performance: " + vehicle.Performance);
            System.Diagnostics.Debug.WriteLine("suspensions: " + vehicle.Suspensions);
            System.Diagnostics.Debug.WriteLine("unlock level: " + vehicle.UnlockCost);
            System.Diagnostics.Debug.WriteLine("image position: {0},{1},{2},{3}", vehicle.ImagePosition.Bottom, vehicle.ImagePosition.Top, vehicle.ImagePosition.Left, vehicle.ImagePosition.Right);
            System.Diagnostics.Debug.WriteLine("body position: {0},{1},{2},{3}", vehicle.BodyPosition.Bottom, vehicle.BodyPosition.Top, vehicle.BodyPosition.Left, vehicle.BodyPosition.Right);
            System.Diagnostics.Debug.WriteLine("wheel position: {0},{1},{2},{3}", vehicle.WheelsPosition[0].Bottom, vehicle.WheelsPosition[0].Top, vehicle.WheelsPosition[0].Left, vehicle.WheelsPosition[0].Right);
            System.Diagnostics.Debug.WriteLine("");
        }

        public static VehicleContainerModel GetVehicles () 
        {
            LoadConfig();
            return vehicleContainer;
        }
    }
}
