using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared {
    // Level manager - Type, upgrades, etc.
    class LevelManager : ILevelManager, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public LevelManager() {
            InitLevelManager();
        }

        void InitLevelManager() {
            if(Levels != null)
                return;
            Levels = JsonReaderLevels.GetLevelsConfig();
        }

        public static LevelManager Instance { get; } = new LevelManager();

        public int LevelCount {
            get => Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.LevelCount.Value, 0);
            set {
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.LevelCount.Value, value);
                OnPropertyChanged();
            }
        }

        public int SelectedLevelId {
            get => Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.IdLevel.Value, -1);
            set {
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.IdLevel.Value, value);
                OnPropertyChanged();
            }
        }

        public LevelModel SelectedLevelModel {
            get {
                var levels = Levels;
                return levels.LevelModel.First(i => i.IdLevel == SelectedLevelId);
            }
            set {
                var levels = Levels;
                var listItem = levels.LevelModel.First(i => i.IdLevel == value.IdLevel);
                var index = levels.LevelModel.IndexOf(listItem);

                if(index != -1) {
                    levels.LevelModel[index] = value;
                    Levels = levels;
                    OnPropertyChanged("Levels");
                }
            }
        }

        public LevelsContainerModel Levels {
            get 
            {
                var json = Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.Levels.Value, "");
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<LevelsContainerModel>(json);
            }
            set 
            {
                var json = JsonConvert.SerializeObject(value);
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.Levels.Value, json);
                OnPropertyChanged();
            }
            //get => JsonReaderLevels.GetLevelsConfig();
            //set => JsonReaderLevels.SaveLevelsConfig(value);
        }

        public LevelModel LoadSingleLevel (int levelId){
            var level = Levels.LevelModel.FirstOrDefault(l => l.IdLevel == levelId);
            SelectedLevelModel = level;
            return level;
        }

        public LastPlayedLevel LastPlayedLevelInfo {
            get {
                var json = Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.LastPlayedLevel.Value, "");
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<LastPlayedLevel>(json);
            }
            set {
                var json = JsonConvert.SerializeObject(value);
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.LastPlayedLevel.Value, json);
                OnPropertyChanged();
            }
        }

        public LoadingScreenData LoadingScreenFacts { get => JsonReaderLoadingScreen.GetLoadingScreenData(); }

        public int PlayedLevels {
            get 
            {
                var played = 0;
                foreach(var lvl in Levels.LevelModel) {
                    played += lvl.TotalOfPlays;
                }
                return played;
            }
        }

        public int CompletedLevels {
            get 
            {
                var completed = 0;
                foreach(var lvl in Levels.LevelModel) {
                    completed += lvl.Completed;
                }
                return completed;
            }
        }

        public int FailedLevels {
            get 
            {
                var failed = 0;
                foreach(var lvl in Levels.LevelModel) {
                    failed += lvl.TotalOfFailures;
                }
                return failed;
            }
        }

        public double CompletedPercentage {
            get {
                double failed = 0;
                double completed = 0;
                foreach(var lvl in Levels.LevelModel) {
                    failed += lvl.TotalOfFailures;
                    completed += lvl.TotalOfPlays - failed;
                }
                if(completed.CompareTo(0) == 0)
                    return 0;
                if(failed.CompareTo(0) == 0)
                    return 100;

                return completed / failed * 100;
            }
        }

        public int MostPointsInSingleRace {
            get {
                var points = 0;
                foreach(var lvl in Levels.LevelModel) {
                    points = lvl.PointsObtained > points ? lvl.PointsObtained : points;
                }
                return points;
            }
        }
    }
}
