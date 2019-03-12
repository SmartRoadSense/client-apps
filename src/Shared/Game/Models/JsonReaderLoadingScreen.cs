using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared {
    public static class JsonReaderLoadingScreen {
        const string LevelJsonFilename = "LoadingScreenData.json";

        static LoadingScreenData LoadConfig() {
            LoadingScreenData levelContainer = null;
            try {
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(App));
                string[] resourceNames = assembly.GetManifestResourceNames();
                var fullname = (from r in resourceNames where r.EndsWith(LevelJsonFilename, StringComparison.Ordinal) select r).FirstOrDefault();

                using(var s = assembly.GetManifestResourceStream(fullname)) {
                    using(var reader = new StreamReader(s)) {
                        var txt = reader.ReadToEnd();
                        JsonSerializer serializer = new JsonSerializer();
                        levelContainer = JsonConvert.DeserializeObject<LoadingScreenData>(txt);
                        return levelContainer;
                    }
                }
            }
            catch(Exception e) {
                System.Diagnostics.Debug.WriteLine("Error decoding level file: " + e);
                return levelContainer;
            }
        }
       
        public static LoadingScreenData GetLoadingScreenData() {
            return LoadConfig();
        }
    }
}
