using System;
namespace SmartRoadSense.Shared {
    public interface ISettingsManager {
        ControllerPosition ControllerLayout { get; set; }
        GameplayButtonSize GameplayButtonSize { get; set; }
    }
}
