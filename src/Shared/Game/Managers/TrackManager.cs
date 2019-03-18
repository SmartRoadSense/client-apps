using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared {
    // Level manager - Type, upgrades, etc.
    class TrackManager : ITrackManager, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TrackManager() {
            InitTrackManager();
        }

        void InitTrackManager() {
            if(Tracks != null)
                return;
            Tracks = JsonReaderLevels.GetLevelsConfig();
        }

        public static TrackManager Instance { get; } = new TrackManager();

        public int TrackCount {
            get => Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.TrackCount.Value, 0);
            set {
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.TrackCount.Value, value);
                OnPropertyChanged();
            }
        }

        public int SelectedTrackId {
            get => Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.IdTrack.Value, -1);
            set {
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.IdTrack.Value, value);
                OnPropertyChanged();
            }
        }

        public TrackModel SelectedTrackModel {
            get {
                var tracks = Tracks;
                return tracks.TrackModel.First(i => i.IdTrack == SelectedTrackId);
            }
            set {
                var tracks = Tracks;
                var listItem = tracks.TrackModel.First(i => i.IdTrack == value.IdTrack);
                var index = tracks.TrackModel.IndexOf(listItem);

                if(index != -1) {
                    tracks.TrackModel[index] = value;
                    Tracks = tracks;
                    OnPropertyChanged("Levels");
                }
            }
        }

        public TracksContainerModel Tracks {
            get 
            {
                var json = Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.Levels.Value, "");
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<TracksContainerModel>(json);
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

        public TrackModel LoadSingleLevel (int levelId){
            var level = Tracks.TrackModel.FirstOrDefault(l => l.IdTrack == levelId);
            SelectedTrackModel = level;
            return level;
        }

        public LastPlayedTrack LastPlayedTrackInfo {
            get {
                var json = Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.LastPlayedLevel.Value, "");
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<LastPlayedTrack>(json);
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
                foreach(var lvl in Tracks.TrackModel) {
                    played += lvl.TotalOfPlays;
                }
                return played;
            }
        }

        public int CompletedLevels {
            get 
            {
                var completed = 0;
                foreach(var lvl in Tracks.TrackModel) {
                    completed += lvl.Completed;
                }
                return completed;
            }
        }

        public int FailedLevels {
            get 
            {
                var failed = 0;
                foreach(var lvl in Tracks.TrackModel) {
                    failed += lvl.TotalOfFailures;
                }
                return failed;
            }
        }

        public double CompletedPercentage {
            get {
                double failed = 0;
                double completed = 0;
                foreach(var lvl in Tracks.TrackModel) {
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
                foreach(var lvl in Tracks.TrackModel) {
                    points = lvl.PointsObtained > points ? lvl.PointsObtained : points;
                }
                return points;
            }
        }
    }
}
