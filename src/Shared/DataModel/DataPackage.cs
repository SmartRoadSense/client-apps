using System;
using System.Collections.Generic;
using System.Text;

namespace SmartRoadSense.Shared.DataModel {

    /// <summary>
    /// Package of data points and metadata stored on disk.
    /// </summary>
    public class DataPackage {

        public DataPackageInfo Info;

        public IList<Data.DataPiece> Pieces;

    }

}
