using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using SmartRoadSense.Toolkit.Parameters;

namespace SmartRoadSense.Toolkit {

    internal static class CompressionRun {

        public static int Execute(CompressionRunParameters p) {
            using (var file = new FileStream(p.Source, FileMode.Open)) {
                using (var output = new FileStream(p.Output, FileMode.Create)) {
                    Stream input;
                    if (p.Decompress)
                        input = new GZipStream(file, CompressionMode.Decompress);
                    else
                        input = new GZipStream(file, CompressionMode.Compress);

                    input.CopyTo(output);
                    output.Flush();

                    return 0;
                }
            }
        }

    }

}
