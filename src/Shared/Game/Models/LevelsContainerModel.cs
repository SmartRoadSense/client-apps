using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared {
    public class TracksContainerModel {
        [JsonProperty("LEVELS")]
        public List<TrackModel> TrackModel { get; set; }
    }

    public class TrackModel {
        [JsonProperty("ID_LEVEL")]
        public int IdTrack { get; set; }

        [JsonProperty("NAME")]
        public string Name { get; set; }

        [JsonProperty("DIFFICULTY")]
        public int Difficulty { get; set; }

        [JsonProperty("TRACK_LENGTH")]
        public int TrackLength { get; set; }

        [JsonProperty("LANDSKAPE")]
        public int Landskape { get; set; } //0 - random, 1 - beach, 2 - moon, 3 - forest

        [JsonProperty("COMPLETED")]
        public int Completed { get; set; }

        [JsonProperty("BEST_TIME")]
        public int BestTime { get; set; }

        [JsonProperty("TOTAL_OF_PLAYS")]
        public int TotalOfPlays { get; set; }

        [JsonProperty("TOTAL_OF_FAILURES")]
        public int TotalOfFailures { get; set; }

        [JsonProperty("TOTAL_POINTS")]
        public int TotalPoints { get; set; }

        [JsonProperty("POINTS_OBTAINED")]
        public int PointsObtained { get; set; }

    }
}
