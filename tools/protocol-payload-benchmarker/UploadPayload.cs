using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolPayloadBenchmark {

    /// <summary>
    /// Internal POCO class used to serialize data to JSON.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class UploadPayload {

        [JsonProperty("bearing")]
        public double Bearing { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("position-resolution")]
        public int PositionResolution { get; set; }

        [JsonProperty("position")]
        public Point Position { get; set; }

        [JsonProperty("values")]
        public double[] Values { get; set; }

        private static Random _rnd = new Random();

        private static readonly long DefaultDuration = TimeSpan.FromSeconds(1.0 / 100.0).Ticks;

        public static UploadPayload GenerateRandom() {
            return new UploadPayload {
                Bearing = _rnd.NextDouble() * 360.0,
                Time = DateTime.UtcNow,
                Duration = (int)DefaultDuration,
                PositionResolution = (int)(_rnd.NextDouble() * 100),
                Position = new Point(new GeographicPosition(0, 0)),
                Values = new double[] {
                    _rnd.NextDouble(),
                    _rnd.NextDouble(),
                    _rnd.NextDouble(),
                    _rnd.NextDouble(),
                    _rnd.NextDouble()
                }
            };
        }

    }

}
