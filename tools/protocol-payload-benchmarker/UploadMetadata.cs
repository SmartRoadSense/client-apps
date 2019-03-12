using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolPayloadBenchmark {

    /// <summary>
    /// Internal POCO class used to serialize metadata to JSON.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class UploadMetadata {

        [JsonProperty("appVersion")]
        public Version ApplicationVersion { get; set; }

        [JsonProperty("operatingSystem")]
        public string OperatingSystem { get; set; }

        [JsonProperty("operatingSystemVersion")]
        public Version OperatingSystemVersion { get; set; }

        [JsonProperty("sdk")]
        public string Sdk { get; set; }

        [JsonProperty("deviceManufacturer")]
        public string DeviceManufacturer { get; set; }

        [JsonProperty("deviceModel")]
        public string DeviceModel { get; set; }

    }

}
