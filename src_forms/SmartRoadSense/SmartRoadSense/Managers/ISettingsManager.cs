using System;
namespace SmartRoadSense
{
    public interface ISettingsManager
    {
        AnchorageType CurrentAnchorageType { get; set; }
        VehicleType CurrentVehicleType { get; set; }
    }
}
