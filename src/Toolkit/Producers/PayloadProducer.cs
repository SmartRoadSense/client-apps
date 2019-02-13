using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using SmartRoadSense.Shared;
using SmartRoadSense.Shared.Api;
using SmartRoadSense.Shared.Data;
using SmartRoadSense.Shared.DataModel;
using SmartRoadSense.Toolkit.Parameters;

namespace SmartRoadSense.Toolkit.Producers {

    internal class PayloadProducer : OutputProducer<PayloadParameters> {

        public PayloadProducer(PayloadParameters parameters)
            : base(parameters) {
        }

        protected override void ProcessChunkOutput(OutputWrapper output, int index, int count, IEnumerable<DataPiece> pieces) {
            if(!output.HasRawStream) {
                throw new ArgumentException("Payload cannot be written to standard output");
            }

            var chunkQuery = new UploadDataQuery();
            chunkQuery.Package = new DataPackage {
                Pieces = pieces.ToList()
            };
            chunkQuery.SecretHash = Crypto.GenerateSecret().ToSha512Hash();

            if (Parameters.GZip) {
                chunkQuery.Compression = CompressionPolicy.RequestGZip;
            }
            else {
                chunkQuery.Compression = CompressionPolicy.Disabled;
            }

            chunkQuery.OverrideHttpClient = new HttpClient(new WriteOutHttpMessageHandler(output.RawStream));

            chunkQuery.Execute(CancellationToken.None).Wait();
        }

    }

}
