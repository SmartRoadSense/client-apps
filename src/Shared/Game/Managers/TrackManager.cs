using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartRoadSense.Shared.Data;

namespace SmartRoadSense.Shared {
    // Level manager - Type, upgrades, etc.
    class TrackManager : ITrackManager, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        Random _random = new Random();
        public static readonly int _minTrackLength = 900;
        public static readonly int _maxTrackLength = 3600;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static TrackManager Instance { get; } = new TrackManager();

        public async Task<bool> Init() {
            return await InitTrackManager();
        }

        async Task<bool> InitTrackManager() {
            try {
                // Get default level
                if(Tracks == null || Tracks.TrackModel.Count == 0) {
                    var model = new TracksContainerModel {
                        TrackModel = new List<TrackModel>()
                    };
                    model.TrackModel.Add(new TrackModel {
                        IdTrack = 0,
                        GuidTrack = new Guid(),
                        Name = "Random track",
                        Difficulty = 1,
                        TrackLength = 3600,
                        Landskape = 0,
                        Completed = 0,
                        BestTime = 0,
                        TotalOfPlays = 0,
                        TotalOfFailures = 0,
                        TotalPoints = 0,
                        PointsObtained = 0
                    });
                    Tracks = model;
                }
                //Tracks = JsonReaderLevels.GetLevelsConfig();

                // Get SRS levels and convert data to game model
                var srsTracks = await DataStore.GetCollectedTracks();
                var currentTracks = Tracks;

                foreach(var t in srsTracks) {
                    // Don't import any tracks with null distance
                    if(t.RecordingDistance <= 0)
                        continue;

                    // Don't import any tracks that are shorter than 
                    var points = await DataStore.GetTrackPpe(t.Id);
                    Debug.WriteLine($"TRACK DATA: {t.Id}: {t.RecordedOn} - {t.RecordingDistance} - {t.RecordingLength}, {points.Length} ppe points.");

                    if(points.Length < 900)
                        continue;

                    // If track is longer than 3600 points, divide it into smaller tracks
                    //if(points.Length > 3600) {
                        var exists = currentTracks.TrackModel.Exists(track => track.GuidTrack == t.Id);
                        if(exists)
                            continue;

                        //var tracks = (double)points.Length / 3600;
                       //tracks = Math.Truncate(tracks);
                    //}

                    var model = new TrackModel {
                        IdTrack = currentTracks.TrackModel.Count,
                        GuidTrack = t.Id,
                        Name = "Track " + (currentTracks.TrackModel.Count + 1),
                        Difficulty = CharacterManager.Instance.User.Level,
                        TrackLength = t.RecordingDistance,
                        Landskape = _random.Next(1, 4),
                        Completed = 0,
                        BestTime = 0,
                        TotalOfPlays = 0,
                        TotalOfFailures = 0,
                        TotalPoints = 0,
                        PointsObtained = 0
                    };
                    currentTracks.TrackModel.Add(model);
                }

                Tracks = currentTracks;
                return true;
            } catch (Exception e) {
                Debug.WriteLine("Error initializing tracks: " + e);
                return false;
            }
        }

        public int TrackCount {
            get {
                var json = Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.Tracks.Value, string.Empty);
                return string.IsNullOrEmpty(json) ? 0 : JsonConvert.DeserializeObject<TracksContainerModel>(json).TrackModel.Count;
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
                var json = Plugin.Settings.CrossSettings.Current.GetValueOrDefault(CrossSettingsIdentifiers.Tracks.Value, "");
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<TracksContainerModel>(json);
            }
            set 
            {
                var json = JsonConvert.SerializeObject(value);
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(CrossSettingsIdentifiers.Tracks.Value, json);
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
                double total = 0;
                foreach(var lvl in Tracks.TrackModel) {
                    failed += lvl.TotalOfFailures;
                    completed += lvl.TotalOfPlays - lvl.TotalOfFailures;
                    total += lvl.TotalOfPlays;
                }
                if(completed.CompareTo(0) <= 0)
                    return 0;
                if(failed.CompareTo(0)<= 0)
                    return 100;

                return (failed / total) * 100;
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
