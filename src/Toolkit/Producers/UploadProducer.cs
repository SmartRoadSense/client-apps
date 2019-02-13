using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartRoadSense.Shared;
using SmartRoadSense.Shared.Api;
using SmartRoadSense.Shared.Data;
using SmartRoadSense.Shared.DataModel;
using SmartRoadSense.Toolkit.Parameters;
using SmartRoadSense.Toolkit.Producers;

namespace SmartRoadSense.Toolkit.Producers {

    internal class UploadProducer : BaseProducer<UploadParameters> {

        public UploadProducer(UploadParameters parameters)
            : base(parameters) {

        }

        protected override void ProcessChunk(int index, int count, IEnumerable<DataPiece> pieces) {
            var query = new UploadDataQuery();
            query.Package = new DataPackage {
                Pieces = pieces.ToList()
            };
            query.SecretHash = Crypto.GenerateSecret().ToSha512Hash();

            if(Parameters.DisableCompression) {
                query.Compression = CompressionPolicy.Disabled;
            }

            query.Service = Parameters.EndPoint;

            query.Execute(CancellationToken.None).Wait();
        }

    }

}
