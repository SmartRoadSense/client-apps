using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoadSense.Shared.Data;
using SmartRoadSense.Toolkit.Parameters;

namespace SmartRoadSense.Toolkit.Producers {

    internal abstract class OutputProducer<TParams> : BaseProducer<TParams>
        where TParams : OutputParameters {

        public OutputProducer(OutputParameters parameters)
            : base(parameters) {

        }

        private OutputWrapper OpenOutputFile(int index, int total) {
            string basePattern = Parameters.OutputFile;

            if (string.IsNullOrWhiteSpace(basePattern)) {
                return new OutputWrapper(Console.Out);
            }
            else {
                string targetFilePath;
                if (total == 1) {
                    targetFilePath = basePattern;
                }
                else {
                    var filename =
                        Path.GetFileNameWithoutExtension(basePattern) +
                        "-" +
                        (index + 1).ToString("D" + total.ToString().Length) +
                        Path.GetExtension(basePattern);

                    targetFilePath = Path.Combine(Path.GetDirectoryName(basePattern), filename);
                }

                var fileStream = new FileStream(targetFilePath, FileMode.Create);

                return new OutputWrapper(fileStream);
            }
        }

        protected override void ProcessChunk(int index, int count, IEnumerable<DataPiece> pieces) {
            using (var output = OpenOutputFile(index, count)) {
                ProcessChunkOutput(output, index, count, pieces);

                //If no raw stream present, we are writing to std.output
                //An ending newline is much appreciated
                if(!output.HasRawStream) {
                    output.Writer.WriteLine();
                }
            }
        }

        protected abstract void ProcessChunkOutput(OutputWrapper output, int index, int count, IEnumerable<DataPiece> pieces);

    }

}
