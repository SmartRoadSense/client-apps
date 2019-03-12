using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartRoadSense.Shared {
    public class SettingsManager : ISettingsManager, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static SettingsManager Instance { get; } = new SettingsManager();

        public ControllerPosition ControllerLayout {
            get =>(ControllerPosition)Plugin.Settings.CrossSettings.Current.GetValueOrDefault("settings_controller_position", 0);
            set 
            {
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("settings_controller_position", (int)value);
                OnPropertyChanged();
            }
        }

        public GameplayButtonSize GameplayButtonSize {
            get => (GameplayButtonSize)Plugin.Settings.CrossSettings.Current.GetValueOrDefault("settings_button_size", 2);
            set {
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("settings_button_size", (int)value);
                OnPropertyChanged();
            }
        }
    }

    public enum ControllerPosition {
        RIGHT_CONTROLLER = 0,
        LEFT_CONTROLLER = 1
    }

    public enum GameplayButtonSize {
        SMALL = 0,
        MEDIUM = 1,
        LARGE = 2
    }
}
