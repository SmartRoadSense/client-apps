using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartRoadSense.Shared {
    public class SoundManager : ISoundManager, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static SoundManager Instance { get; } = new SoundManager();

        public float MusicGain {
            get => Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.Music.Value, 0.75f);
            set => Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.Music.Value, value);
        }

        public float EffectsGain {
            get => Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.SoundEffects.Value, 0.75f);
            set => Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.SoundEffects.Value, value);
        }
    }
}
