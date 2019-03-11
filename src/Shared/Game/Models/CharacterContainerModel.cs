using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;


namespace SmartRoadSense.Shared {
    public class CharacterContainerModel {
        [JsonProperty("CHARACTERS")]
        public List<CharacterModel> CharacterModel { get; set; }
    }

    public class CharacterModel {
        [JsonProperty("ID_CHARACTER")]
        public int IdCharacter { get; set; }

        [JsonProperty("TYPE")]
        public int Type { get; set; }

        [JsonProperty("IMAGE_POSITION")]
        public CharacterImagePosition ImagePosition { get; set; }

    }


    public class CharacterImagePosition {
        [JsonProperty("LEFT")]
        public int Left { get; set; }

        [JsonProperty("TOP")]
        public int Top { get; set; }

        [JsonProperty("RIGHT")]
        public int Right { get; set; }

        [JsonProperty("BOTTOM")]
        public int Bottom { get; set; }
    }
}
