using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared {

    public static class JsonSerializationExtensions {

        /// <summary>
        /// Serializes an object to a JSON string using default settings.
        /// </summary>
        public static string ToJson(this object obj) {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }

        /// <summary>
        /// Serializes an object to string using a <see cref="JsonSerializer"/>.
        /// </summary>
        public static string SerializeToString(this JsonSerializer serializer, object obj) {
            //TODO this could be improved performance-wise
            var sb = new StringBuilder();

            using(var writer = new JsonTextWriter(new StringWriter(sb, CultureInfo.InvariantCulture))) {
                serializer.Serialize(writer, obj);
            }

            return sb.ToString();
        }

    }

}
