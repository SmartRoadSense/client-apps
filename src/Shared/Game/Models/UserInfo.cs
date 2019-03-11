using System;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared {
    public class UserInfo {

        [JsonProperty("USERNAME")]
        public string Username { get; set; }

        [JsonProperty("PORTRAIT_ID")]
        public int PortraitId { get; set; }

        [JsonProperty("WALLET")]
        public int Wallet { get; set; }

        [JsonProperty("EXPERIENCE")]
        public int Experience { get; set; }

        [JsonProperty("LAST_COMPLEDED_RACE")]
        public int LastCompletedRace { get; set; }

        [JsonIgnore]
        public int Level => CharacterLevelData.CurrentUserLevel();
    }
}
