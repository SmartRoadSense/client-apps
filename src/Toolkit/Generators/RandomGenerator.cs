using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoadSense.Shared.Data;

namespace SmartRoadSense.Toolkit.Generators {

    internal class RandomGenerator : BaseGenerator {

        private readonly TimeSpan PieceInterval = TimeSpan.FromSeconds(1);

        private const double DefaultLatitude = 43.726368;
        private const double DefaultLongitude = 12.636313;
        private const double DistanceInterval = 0.00015; // approx. 45 km/h

        public override IEnumerable<DataPiece> Generate() {
            var rnd = new Random(Parameters.RandomSeed);
            Program.VerboseLog("Random generator initialized with seed {0}.", Parameters.RandomSeed);

            Guid trackId = Guid.NewGuid();
            Program.VerboseLog("Track ID: {0:B}.", trackId);
            Program.Stats.AddTrackId(trackId);

            DateTime timestamp = DateTime.UtcNow;

            double lat = DefaultLatitude;
            double lng = DefaultLongitude;

            double orientation = rnd.NextDouble();

            while (true) {
                Program.Stats.AddInputPoint();

                yield return GenerateDataPiece(rnd, trackId, timestamp, lat, lng);

                timestamp = timestamp.Add(PieceInterval);

                var orSin = Math.Sin(orientation);
                lat += DistanceInterval * orSin;
                lng += DistanceInterval * (1 - orSin);

                orientation += (rnd.NextDouble() - 0.5);
            }
        }

        private DataPiece GenerateDataPiece(Random rnd, Guid trackId, DateTime timestamp, double lat, double lng) {
            var x = rnd.NextDouble();
            var y = rnd.NextDouble();
            var z = rnd.NextDouble();

            var ret = new DataPiece {
                TrackId = trackId,
                StartTimestamp = timestamp,
                EndTimestamp = timestamp.Add(PieceInterval),
                PpeX = x,
                PpeY = y,
                PpeZ = z,
                Ppe = (x + y + z) / 3.0,
                Latitude = lat,
                Longitude = lng,
                Accuracy = (int)(rnd.NextDouble() * 30),
                Bearing = 0f,
                Speed = 0,
                Anchorage = Shared.AnchorageType.MobileBracket,
                Vehicle = Shared.VehicleType.Car
            };

            return ret;
        }

    }

}
