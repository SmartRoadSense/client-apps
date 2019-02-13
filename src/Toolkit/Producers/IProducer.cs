using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRoadSense.Toolkit.Producers {

    internal interface IProducer {

        void Process(IList<IEnumerable<Shared.Data.DataPiece>> chunks);

    }

}
