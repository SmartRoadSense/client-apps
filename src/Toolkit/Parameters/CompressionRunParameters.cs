using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SmartRoadSense.Toolkit.Parameters {

    [Verb("gzip",
        HelpText = "On-the-fly GZip file compression and decompression."
    )]
    internal class CompressionRunParameters {

        [Option('s', "source", HelpText = "Input file.")]
        public string Source { get; set; }

        [Option('d', "decompress", HelpText = "Decompresses the input file.")]
        public bool Decompress { get; set; }

        [Option('o', "output", HelpText = "Output file.")]
        public string Output { get; set; }

    }

}
