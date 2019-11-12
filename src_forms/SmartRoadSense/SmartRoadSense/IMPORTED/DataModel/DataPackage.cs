using System.Collections.Generic;

namespace SmartRoadSense {

    /// <summary>
    /// Package of data points and metadata stored on disk.
    /// </summary>
    public class DataPackage {

        public DataPackageInfo Info;

        public IList<DataPiece> Pieces;

    }

}
