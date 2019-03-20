using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared
{
    // User manager - levels, porfile, etc.
    class CharacterManager : ICharacterManager, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static CharacterManager Instance { get; } = new CharacterManager();

        public UserInfo User {
            get {
                var json = Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.UserInfo.Value, "");
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<UserInfo>(json);
            }
            set {
                var json = JsonConvert.SerializeObject(value);
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.UserInfo.Value, json);
                OnPropertyChanged();
            }
        }

        public int Wallet {
            get => User != null ? User.Wallet : 0;
            set {
                var tmp = User;
                tmp.Wallet = value;
                var json = JsonConvert.SerializeObject(tmp);
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.UserInfo.Value, json);
                OnPropertyChanged();
            }
        }
        /*
        public CharacterModel SelectedCharacterModel {
            get {
                var json = Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.SelectedCharacter.Value, "");
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<CharacterModel>(json);
            }
            set {
                var json = JsonConvert.SerializeObject(value);
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.SelectedCharacter.Value, json);
                OnPropertyChanged();
            }
        }
        */
        public int CharacterCount {
            get => Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.CharacterCount.Value, 0);
            set {
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.CharacterCount.Value, value);
                OnPropertyChanged();
            }
        }
    }
}
