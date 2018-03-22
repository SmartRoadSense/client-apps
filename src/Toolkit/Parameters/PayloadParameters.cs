using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SmartRoadSense.Toolkit.Parameters {

    [Verb("payload",
        HelpText="Generate a raw payload for the API."
    )]
    internal class PayloadParameters : OutputParameters {

        [Option("gzip", HelpText = "Enables GZip compression on output.")]
        public bool GZip { get; set; }

    }

}
