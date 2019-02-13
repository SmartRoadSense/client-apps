using System;
using System.Collections.Generic;
using System.IO;
using SmartRoadSense.Shared.Data;
using SmartRoadSense.Toolkit.Parameters;

namespace SmartRoadSense.Toolkit.Producers {

    internal abstract class BaseProducer<TParams> : IProducer
        where TParams : CommonParameters {

        public BaseProducer(CommonParameters parameters) {
            CommonParameters = parameters;
        }

        private CommonParameters CommonParameters { get; set; }

        protected TParams Parameters {
            get {
                return (TParams)CommonParameters;
            }
        }

        public virtual void Process(IList<IEnumerable<DataPiece>> chunks) {
            int chunkCount = chunks.Count;

            for (int i = 0; i < chunkCount; ++i) {
                Program.VerboseLog("Processing chunk {0}...", i + 1);

                this.ProcessChunk(i, chunkCount, chunks[i]);

                Program.Stats.AddChunk();
            }
        }

        protected abstract void ProcessChunk(int index, int count, IEnumerable<DataPiece> pieces);

    }

}
