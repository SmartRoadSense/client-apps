using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SmartRoadSense.Toolkit.Parameters {

    internal class CommonParameters {

        [Option('v', "verbose", HelpText = "Outputs debug information to standard output.")]
        public bool Verbose { get; set; }

        [Option('s', "source", SetName = "generator", HelpText = "List of files with serialized data points (as produced by SmartRoadSense applications) to be used as data source.")]
        public IEnumerable<string> SourceFiles { get; set; }

        [Option("random", SetName = "generator", HelpText = "Uses a random number generator to produce data points.")]
        public bool RandomGenerator { get; set; }

        [Option("seed", HelpText = "Seed for random generator.")]
        public int RandomSeed { get; set; }

        [Option('n', "length", HelpText = "Trims input to a fixed amount of data points.")]
        public int? Length { get; set; }

        [Option("chunking", HelpText = "Chunks the data set into groups of a certain number of points.")]
        public int? Chunking { get; set; }

        [Option("skip", HelpText = "Skips n points of the input sequence.")]
        public int? Skip { get; set; }

        [Option("every", HelpText = "Takes every n-th element of the input sequence (after skipping).")]
        public int? Every { get; set; }

        [Option("track-id", HelpText = "Sets one track ID for all data points.")]
        public string AmendTrackIdString { get; set; }

        internal Guid? AmendTrackId {
            get {
                if (string.IsNullOrWhiteSpace(AmendTrackIdString))
                    return null;
                else
                    return Guid.Parse(AmendTrackIdString);
            }
            set {
                if (value.HasValue)
                    AmendTrackIdString = value.ToString();
                else
                    AmendTrackIdString = null;
            }
        }

        [Option("one-track", HelpText = "Extends the first track ID to all data points.")]
        public bool OneTrackId { get; set; }

        [Option("update-timestamps", HelpText = "Updates all timestamps to progressive times starting from yesterday.")]
        public bool UpdateTimestamps { get; set; }

    }

}
