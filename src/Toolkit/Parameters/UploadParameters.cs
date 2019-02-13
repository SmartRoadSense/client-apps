using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SmartRoadSense.Toolkit.Parameters {

    [Verb("upload",
        HelpText = "Uploads data to the SmartRoadSense aggregation service."
    )]
    internal class UploadParameters : CommonParameters {

        [Option("disable-compression", HelpText = "Disabled compression for both HTTP request and response.")]
        public bool DisableCompression { get; set; }

        [Option('e', "endpoint", Default = Shared.Api.ServiceType.Development,
            HelpText = "The type of service endpoint to use (Production or Development)."
        )]
        public Shared.Api.ServiceType EndPoint { get; set; }

    }

}
