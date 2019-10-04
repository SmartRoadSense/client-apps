using System;
using System.Collections.Generic;

namespace SmartRoadSense
{
    // DATA SOURCE
    public enum VehicleSource
    {
        CAR,
        MOTORCYCLE,
        BUS_TRUCK
    }

    public enum AnchorageSource
    {
        MOUNT,
        MAT,
        POCKET
    }

    public static class SettingsViewSource
    {
        public static Dictionary<VehicleSource, string> VehicleTypeStrings = new Dictionary<VehicleSource, string>
        {
            { VehicleSource.CAR, AppResources.CarLabel },
            { VehicleSource.MOTORCYCLE, AppResources.MotorcycleLabel },
            { VehicleSource.BUS_TRUCK, AppResources.TruckLabel }
        };

        public static Dictionary<VehicleSource, string> VehicleIconSource = new Dictionary<VehicleSource, string>
        {
            { VehicleSource.CAR, "icon_car" },
            { VehicleSource.MOTORCYCLE, "icon_motorcycle" },
            { VehicleSource.BUS_TRUCK, "icon_bus" }
        };

        public static Dictionary<AnchorageSource, string> AnchorageTypeStrings = new Dictionary<AnchorageSource, string>
        {
            { AnchorageSource.MOUNT, AppResources.AnchorageBracketLabel },
            { AnchorageSource.MAT, AppResources.AnchorageMatLabel },
            { AnchorageSource.POCKET, AppResources.AnchoragePocketLabel }
        };

        public static Dictionary<AnchorageSource, string> AnchorageIconSource = new Dictionary<AnchorageSource, string>
        {
            { AnchorageSource.MOUNT, "icon_bracket" },
            { AnchorageSource.MAT, "icon_mat" },
            { AnchorageSource.POCKET, "icon_pocket" }
        };
    }

    
}
