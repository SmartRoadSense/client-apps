using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace SmartRoadSense.Shared.DataModel {

    [JsonObject(MemberSerialization.OptIn)]
    #if __ANDROID__
    [global::Android.Runtime.Preserve(AllMembers = true)]
    #endif
    public class DataPackageInfo {

        //TODO

    }

}
