using System;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared {
    public class LastPlayedLevel {
        [JsonProperty("LEVEL_DATA")]
        public LevelModel LevelData { get; set; }

        [JsonProperty("TIME")]
        public int Time { get; set; }

        [JsonProperty("COMPONENTS")]
        public int Components { get; set; }

        [JsonProperty("COINS")]
        public int Coins { get; set; }

        [JsonProperty("POINTS")]
        public int Points { get; set; }
    }
}
