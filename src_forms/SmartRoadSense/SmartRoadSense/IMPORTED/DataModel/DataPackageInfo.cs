using Newtonsoft.Json;

namespace SmartRoadSense {

    [JsonObject(MemberSerialization.OptIn)]
    #if __ANDROID__
    [global::Android.Runtime.Preserve(AllMembers = true)]
    #endif
    public class DataPackageInfo {

        //TODO

    }

}
