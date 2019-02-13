using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SmartRoadSense.Toolkit.Parameters {

    [Verb("geojson",
        HelpText = "Generate a GeoJSON LineString."
    )]
    internal class GeoJsonParameters : OutputParameters {

        [Option("pretty", HelpText = "Pretty print GeoJSON output.")]
        public bool PrettyPrint { get; set; }

        public enum GeoJsonOutputType {
            LineString,
            MultiPoint
        }

        [Option("type", Default = GeoJsonOutputType.LineString, HelpText = "Type of GeoJSON object to generate (LineString or MultiPoint).")]
        public GeoJsonOutputType Type { get; set; }

    }

}
