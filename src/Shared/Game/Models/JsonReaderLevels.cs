using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared {
    public static class JsonReaderLevels {
        const string LevelJsonFilename = "Levels.json";

        static LevelsContainerModel LoadConfig() {
            LevelsContainerModel levelContainer = null;
            try {
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(App));
                string[] resourceNames = assembly.GetManifestResourceNames();
                var fullname = (from r in resourceNames where r.EndsWith(LevelJsonFilename, StringComparison.Ordinal) select r).FirstOrDefault();

                using(var s = assembly.GetManifestResourceStream(fullname)) {
                    using(var reader = new StreamReader(s)) {
                        var txt = reader.ReadToEnd();
                        JsonSerializer serializer = new JsonSerializer();
                        levelContainer = JsonConvert.DeserializeObject<LevelsContainerModel>(txt);
                        return levelContainer;
                    }
                }
            }
            catch(Exception e) {
                System.Diagnostics.Debug.WriteLine("Error decoding level file: " + e);
                return levelContainer;
            }
        }
        /*
        static void SaveConfig(string jsonText) {
            try {
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(App));
                string[] resourceNames = assembly.GetManifestResourceNames();
                var fullname = (from r in resourceNames where r.EndsWith(LevelJsonFilename, StringComparison.Ordinal) select r).FirstOrDefault();

                using(var s = assembly.GetManifestResourceStream(fullname)) {
                    using(var writer = new StreamWriter(s)) {
                        JsonSerializer serializer = new JsonSerializer();
                        var serialized = JsonConvert.SerializeObject(jsonText);
                        writer.Write(serialized);
                        writer.Flush();
                    }
                }
            }
            catch(Exception e) {
                System.Diagnostics.Debug.WriteLine("Error encoding level file: " + e);
            }
        }
        */
        public static int GetLevelConfig() {
            var levelContainer = LoadConfig();
            if(levelContainer != null) {
                System.Diagnostics.Debug.WriteLine("level count: {0}", levelContainer.LevelModel.Count);
                LevelManager.Instance.LevelCount = levelContainer.LevelModel.Count;
                return levelContainer.LevelModel.Count;
            }
            else
                return 0;
        }

        public static LevelsContainerModel GetLevelsConfig() {
            return LoadConfig();
        }
        /*
        public static void SaveLevelsConfig(LevelsContainerModel jsonObject) {
            var jsonText = JsonConvert.SerializeObject(jsonObject);
            SaveConfig(jsonText);
        }

        public static LevelModel GetSingleLevel(int id) {
            var levelContainer = LoadConfig();
            if(levelContainer != null)
                return levelContainer.LevelModel.FirstOrDefault(levelCont => levelCont.IdLevel == id);
            return null;

            System.Diagnostics.Debug.WriteLine("id: " + level.IdLevel);
            System.Diagnostics.Debug.WriteLine("name: " + level.Name);
            System.Diagnostics.Debug.WriteLine("difficulty: " + level.Difficulty);
            System.Diagnostics.Debug.WriteLine("track_length: " + level.TrackLength);
            System.Diagnostics.Debug.WriteLine("landskape: " + level.Landskape);
            System.Diagnostics.Debug.WriteLine("completed: " + level.Completed);
            System.Diagnostics.Debug.WriteLine("best: " + level.BestTime);
            System.Diagnostics.Debug.WriteLine("total of plays: " + level.TotalOfPlays);
            System.Diagnostics.Debug.WriteLine("total of failures: " + level.TotalOfFailures);
            System.Diagnostics.Debug.WriteLine("total points: " + level.TotalPoints);
            System.Diagnostics.Debug.WriteLine("points obtained: " + level.PointsObtained);
            System.Diagnostics.Debug.WriteLine("");
                       
        }

        public static void UpdateSingleLevel(LevelModel level) {
            var levelContainer = LoadConfig();
            if(levelContainer == null) {
                System.Diagnostics.Debug.WriteLine("Couldn't update level, json file not opened correctly.");
                return;
            }
            var listItem = levelContainer.LevelModel.First(i => i.IdLevel == level.IdLevel);
            var index = levelContainer.LevelModel.IndexOf(listItem);

            if(index != -1) {
                levelContainer.LevelModel[index] = level;
                SaveLevelsConfig(levelContainer);
            }
        }*/
    }
}
