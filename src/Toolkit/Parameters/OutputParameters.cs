using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SmartRoadSense.Toolkit.Parameters {

    internal class OutputParameters : CommonParameters {

        [Option('o', "output", HelpText = "Output file (used as pattern for chunked output).")]
        public string OutputFile { get; set; }

    }

}
