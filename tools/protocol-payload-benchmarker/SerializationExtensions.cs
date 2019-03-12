using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolPayloadBenchmark {

    public static class SerializationExtensions {

        /// <summary>
        /// Serializes an object to a JSON string using default settings.
        /// </summary>
        public static string ToJson(this object obj) {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }

    }

}
