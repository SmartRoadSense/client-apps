using System;
using Newtonsoft.Json;
using System.IO;

namespace SmartRoadSense.Shared {

    public static class JsonExtensions {

        public static T Deserialize<T>(this JsonSerializer json, TextReader reader) {
            return (T)json.Deserialize(reader, typeof(T));
        }

        public static void Serialize<T>(this JsonSerializer json, TextWriter writer, T value) {
            json.Serialize(writer, value, typeof(T));
        }

    }

}

