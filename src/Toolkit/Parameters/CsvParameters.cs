using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SmartRoadSense.Toolkit.Parameters {

    [Verb("csv",
        HelpText = "Generate a CSV data file."
    )]
    internal class CsvParameters : OutputParameters {

    }

}
