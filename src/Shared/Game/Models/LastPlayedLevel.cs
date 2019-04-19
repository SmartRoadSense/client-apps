using Newtonsoft.Json;

namespace SmartRoadSense.Shared {
    public class LastPlayedTrack {
        [JsonProperty("LEVEL_DATA")]
        public TrackModel TrackData { get; set; }

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
