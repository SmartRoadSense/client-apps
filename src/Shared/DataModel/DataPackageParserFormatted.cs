using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared.DataModel {

    /// <summary>
    /// Parses <see cref="DataPackage"/> objects from a stream, using the new formatted
    /// serialization package.
    /// </summary>
    public class DataPackageParserFormatted : DataPackageParser {

        /// <summary>
        /// Extracts a complete <see cref="DataPackage"/> structure from an input stream.
        /// Throws if the package cannot be fully parsed.
        /// </summary>
        public override DataPackage Parse(Stream input) {
            var ret = new DataPackage();

            //StreamReader is left un-disposed (finalization does not close stream)
            var streamReader = new StreamReader(input);

            using (var jsonReader = new JsonTextReader(streamReader)) {
                jsonReader.CloseInput = false;

                ParseObject(jsonReader, new Dictionary<string, Func<JsonTextReader, ParsingContinuation>> {
                    { InformationPropertyName, r => {
                        ret.Info = Json.Deserialize<DataPackageInfo>(r);
                        return ParsingContinuation.Continue;
                    } },
                    { PiecesPropertyName, r => {
                        ret.Pieces = Json.Deserialize<List<Data.DataPiece>>(r);
                        return ParsingContinuation.Continue;
                    } }
                }, r => { });
            }

            if (ret.Info == null || ret.Pieces == null)
                throw new JsonSerializationException("JSON data did not contain package information or data pieces");

            return ret;
        }

        /// <summary>
        /// Extracts a <see cref="DataPackageInfo"/> from an input stream.
        /// Throws if the data cannot be parsed.
        /// </summary>
        public override DataPackageInfo ParseInfo(Stream input) {
            DataPackageInfo ret = null;

            //StreamReader is left un-disposed (finalization does not close stream)
            var streamReader = new StreamReader(input);

            using (var jsonReader = new JsonTextReader(streamReader)) {
                jsonReader.CloseInput = false;

                ParseObject(jsonReader, new Dictionary<string, Func<JsonTextReader, ParsingContinuation>> {
                    { InformationPropertyName, r => {
                        ret = Json.Deserialize<DataPackageInfo>(r);
                        return ParsingContinuation.Stop;
                    } }
                }, r => { });
            }

            if (ret == null)
                throw new JsonSerializationException("JSON data did not contain package information");

            return ret;
        }
    }

}
