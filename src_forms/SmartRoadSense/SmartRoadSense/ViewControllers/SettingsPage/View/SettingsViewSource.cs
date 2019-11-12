using System;
using System.Collections.Generic;

namespace SmartRoadSense
{
    public static class SettingsViewSource
    {
        public static Dictionary<VehicleType, string> VehicleTypeStrings = new Dictionary<VehicleType, string>
        {
            { VehicleType.CAR, AppResources.CarLabel },
            { VehicleType.MOTORCYCLE, AppResources.MotorcycleLabel },
            { VehicleType.BUS_TRUCK, AppResources.TruckLabel }
        };

        public static Dictionary<VehicleType, string> VehicleIconSource = new Dictionary<VehicleType, string>
        {
            { VehicleType.CAR, "icon_car" },
            { VehicleType.MOTORCYCLE, "icon_motorcycle" },
            { VehicleType.BUS_TRUCK, "icon_bus" }
        };

        public static Dictionary<AnchorageType, string> AnchorageTypeStrings = new Dictionary<AnchorageType, string>
        {
            { AnchorageType.MOUNT, AppResources.AnchorageBracketLabel },
            { AnchorageType.MAT, AppResources.AnchorageMatLabel },
            { AnchorageType.POCKET, AppResources.AnchoragePocketLabel }
        };

        public static Dictionary<AnchorageType, string> AnchorageIconSource = new Dictionary<AnchorageType, string>
        {
            { AnchorageType.MOUNT, "icon_bracket" },
            { AnchorageType.MAT, "icon_mat" },
            { AnchorageType.POCKET, "icon_pocket" }
        };
    }

    
}
