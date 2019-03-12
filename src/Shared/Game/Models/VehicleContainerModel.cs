using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared {
    public class VehicleContainerModel 
    {
        [JsonProperty("VEHICLES")]
        public List<VehicleModel> VehicleModel { get; set; }
    }

    public class VehicleModel
    {
        [JsonProperty("ID_VEHICLE")]
        public int IdVehicle { get; set; }

        [JsonProperty("VEHICLE_NAME")]
        public string Name { get; set; }

        [JsonProperty("IMAGE_POSITION")]
        public VehicleImagePosition ImagePosition { get; set; }

        [JsonProperty("BODY_POSITION")]
        public VehicleBodyPosition BodyPosition { get; set; }

        [JsonProperty("WHEELS_POSITION")]
        public List<VehicleWheelsPosition> WheelsPosition { get; set; }

        [JsonProperty("WHEELS_BODY_POSITION")]
        public List<RelativePosition> WheelsBodyPosition { get; set; }

        [JsonProperty("PERFORMANCE")]
        public int Performance { get; set; }

        [JsonProperty("SUSPENSIONS")]
        public int Suspensions { get; set; }

        [JsonProperty("WHEEL")]
        public int Wheel { get; set; }

        [JsonProperty("BRAKE")]
        public int Brake { get; set; }

        [JsonProperty("UNLOCK_COST")]
        public int UnlockCost { get; set; }
    }

    public class VehicleImagePosition
    {
        [JsonProperty("LEFT")]
        public int Left { get; set; }

        [JsonProperty("TOP")]
        public int Top { get; set; }

        [JsonProperty("RIGHT")]
        public int Right { get; set; }

        [JsonProperty("BOTTOM")]
        public int Bottom { get; set; }
    }

    public class VehicleBodyPosition {
        [JsonProperty("LEFT")]
        public int Left { get; set; }

        [JsonProperty("TOP")]
        public int Top { get; set; }

        [JsonProperty("RIGHT")]
        public int Right { get; set; }

        [JsonProperty("BOTTOM")]
        public int Bottom { get; set; }
    }

    public class VehicleWheelsPosition {
        [JsonProperty("LEFT")]
        public int Left { get; set; }

        [JsonProperty("TOP")]
        public int Top { get; set; }

        [JsonProperty("RIGHT")]
        public int Right { get; set; }

        [JsonProperty("BOTTOM")]
        public int Bottom { get; set; }
    }

    public class RelativePosition {
        [JsonProperty("X")]
        public int X { get; set; }

        [JsonProperty("Y")]
        public int Y { get; set; }
    }
}
