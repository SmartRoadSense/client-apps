using System;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared {

    public static class JsonWriterExtensions {

        /// <summary>
        /// Writes a coordinates pair to a JSON writer.
        /// </summary>
        public static void WriteCoordinates(this JsonWriter writer, double longitude, double latitude) {
            writer.WriteStartObject();

            writer.WritePropertyName("type");
            writer.WriteValue("Point");

            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            writer.WriteValue(longitude);
            writer.WriteValue(latitude);
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        /// <summary>
        /// Writes a duration to a JSON writer, as number of milliseconds between two events.
        /// </summary>
        public static void WriteDuration(this JsonWriter writer, DateTime start, DateTime end) {
            if (start > end) {
                var tmp = start;
                start = end;
                end = tmp;
            }

            var span = end - start;
            writer.WriteValue(span.TotalMilliseconds);
        }

    }

}

