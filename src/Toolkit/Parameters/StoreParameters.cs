using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SmartRoadSense.Toolkit.Parameters {

    [Verb("store",
        HelpText = "Generate a serialized storage file."
    )]
    internal class StoreParameters : OutputParameters {

        [Option("pretty", HelpText = "Pretty print JSON output.")]
        public bool PrettyPrint { get; set; }

    }

}
