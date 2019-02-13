using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoadSense.Shared.Data;
using SmartRoadSense.Toolkit.Parameters;

namespace SmartRoadSense.Toolkit.Producers {

    internal class CsvProducer : OutputProducer<CsvParameters> {

        public CsvProducer(CsvParameters parameters)
            : base(parameters) {

        }

        protected override void ProcessChunkOutput(OutputWrapper output,
            int index, int count, IEnumerable<DataPiece> pieces) {

            output.Writer.WriteLine("Track;Start;End;Ppe;Speed;Lat;Lng;");
            foreach(var p in pieces) {
                output.Writer.WriteLine("{0:D};{1};{2};{3};{4};{5};{6};",
                    p.TrackId,
                    p.StartTimestamp.Ticks,
                    p.EndTimestamp.Ticks,
                    p.Ppe,
                    p.Speed,
                    p.Latitude,
                    p.Longitude
                );
            }
        }

    }
}
