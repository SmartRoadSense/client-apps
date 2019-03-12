using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolPayloadBenchmark {

    /// <summary>
    /// POCO data structure that hold information about a recorded data point.
    /// Can be serialized to disk.
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class DataPiece {

        public Guid TrackId;

        public DateTime StartTimestamp;
        public DateTime EndTimestamp;

        public double Ppe;
        public double PpeX, PpeY, PpeZ;
        public double Speed;

        public double Latitude, Longitude;

        public float Bearing;
        public int Accuracy;

        public int Vehicle;
        public int Anchorage;

        private static Random _rnd = new Random();

        public static DataPiece GenerateRandom() {
            return new DataPiece {
                TrackId = Guid.Empty,
                StartTimestamp = DateTime.UtcNow,
                EndTimestamp = DateTime.UtcNow,
                Ppe = _rnd.NextDouble(),
                PpeX = _rnd.NextDouble(),
                PpeY = _rnd.NextDouble(),
                PpeZ = _rnd.NextDouble(),
                Latitude = _rnd.NextDouble(),
                Longitude = _rnd.NextDouble(),
                Bearing = (float)_rnd.NextDouble(),
                Accuracy = _rnd.Next(100),
                Vehicle = 1,
                Anchorage = 1
            };
        }
    }

}
