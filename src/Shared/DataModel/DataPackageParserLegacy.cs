using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartRoadSense.Shared.DataModel {

    /// <summary>
    /// Parses <see cref="DataPackage"/> objects from a stream, using the legacy serialization
    /// format.
    /// </summary>
    /// <remarks>
    /// The legacy serialization format is composed of a raw JSON array of <see cref="DataPiece"/>
    /// objects, without additional information.
    /// This format was used up to SmartRoadSense 3.0.2 in production.
    /// </remarks>
    public class DataPackageParserLegacy : DataPackageParser {

        public override DataPackage Parse(Stream input) {
            //StreamReader is left un-disposed (finalization does not close stream)
            var streamReader = new StreamReader(input);

            var pieces = Json.Deserialize<List<Data.DataPiece>>(streamReader);

            return new DataPackage {
                Info = new DataPackageInfo(),
                Pieces = pieces
            };
        }

        public override DataPackageInfo ParseInfo(Stream input) {
            return new DataPackageInfo();
        }

    }

}
