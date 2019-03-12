using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared {
    public class LoadingScreenData {
        [JsonProperty("loadingScreen")]
        public List<LoadingScreenPage> LoadingScreens { get; set; }
    }

    public class LoadingScreenPage {
        [JsonProperty("type")]
        public int TypeId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("facts")]
        public List<Fact> Facts { get; set; }

        [JsonIgnore]
        public LoadingScreenType Type { get => TypeConverter(TypeId); }

        LoadingScreenType TypeConverter(int id) {
            switch(id) {
                case 0:
                    return LoadingScreenType.ENV;
                case 1:
                    return LoadingScreenType.WILD;
                case 2:
                    return LoadingScreenType.WASTE;
                case 3:
                    return LoadingScreenType.RES;
                case 4:
                    return LoadingScreenType.CLI;
                default:
                    return LoadingScreenType.ENV;
            }
        }
    }

    public class Fact {
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public enum LoadingScreenType {
        ENV,
        WILD,
        WASTE,
        RES,
        CLI
    }
}
