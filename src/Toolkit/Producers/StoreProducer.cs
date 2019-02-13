using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SmartRoadSense.Shared.Data;
using SmartRoadSense.Toolkit.Parameters;

namespace SmartRoadSense.Toolkit.Producers {

    internal class StoreProducer : OutputProducer<StoreParameters> {

        public StoreProducer(StoreParameters parameters)
            : base(parameters) {

            Json = new JsonSerializer();
            if (parameters.PrettyPrint) {
                Json.Formatting = Formatting.Indented;
            }
        }

        private readonly JsonSerializer Json;

        protected override void ProcessChunkOutput(OutputWrapper output, int index, int count, IEnumerable<DataPiece> pieces) {
            Json.Serialize(output.Writer, pieces);
        }

    }
}
