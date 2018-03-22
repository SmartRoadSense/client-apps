using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using SmartRoadSense.Shared.Data;
using SmartRoadSense.Toolkit.Parameters;

namespace SmartRoadSense.Toolkit.Producers {

    internal class GeoJsonProducer : OutputProducer<GeoJsonParameters> {

        public GeoJsonProducer(GeoJsonParameters parameters)
            : base(parameters) {

            Json = new JsonSerializer();
            if (parameters.PrettyPrint) {
                Json.Formatting = Formatting.Indented;
            }
        }

        private readonly JsonSerializer Json;

        protected override void ProcessChunkOutput(OutputWrapper output, int index, int count, IEnumerable<DataPiece> pieces) {
            var positions = from p in pieces
                            select new Position(p.Latitude, p.Longitude);

            object jsonValue = null;
            switch (Parameters.Type) {
                case GeoJsonParameters.GeoJsonOutputType.LineString:
                default:
                    jsonValue = new LineString(positions);
                    break;

                case GeoJsonParameters.GeoJsonOutputType.MultiPoint:
                    jsonValue = new MultiPoint((from p in positions select new Point(p)).ToList());
                    break;
            }

            Json.Serialize(output.Writer, jsonValue);
        }

    }

}
