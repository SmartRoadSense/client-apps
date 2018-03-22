using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared.DataModel {

    /// <summary>
    /// Parses a <see cref="DataPackage"/> object from a stream.
    /// </summary>
    public abstract class DataPackageParser {

        public DataPackageParser() {
            Json = new JsonSerializer();
        }

        protected const string FormatPropertyName = "formatVersion";
        protected const string PiecesPropertyName = "pieces";
        protected const string InformationPropertyName = "info";

        /// <summary>
        /// Attempts to detect the data package parser by reading a stream's contents.
        /// </summary>
        public static DataPackageParser DetectParser(Stream input) {
            //StreamReader is left un-disposed (finalization does not close stream)
            var streamReader = new StreamReader(input);

            using (var jsonReader = new JsonTextReader(streamReader)) {
                jsonReader.CloseInput = false;

                //Check for array of objects, indicating a legacy data package
                ForcedRead(jsonReader);

                if (jsonReader.TokenType == JsonToken.StartArray) {
                    ForcedRead(jsonReader);

                    if (jsonReader.TokenType == JsonToken.StartObject) {
                        //Array of objects, sounds like a legacy package
                        return new DataPackageParserLegacy();
                    }
                }
                else if (jsonReader.TokenType == JsonToken.StartObject) {
                    while (jsonReader.Read()) {
                        if (jsonReader.TokenType == JsonToken.PropertyName) {
                            if (FormatPropertyName.Equals((string)jsonReader.Value)) {
                                //Object with format property, sounds like a new package
                                return new DataPackageParserFormatted();
                            }
                        }

                        jsonReader.Skip();
                    }
                }

                throw new JsonSerializationException(
                    string.Format("Unforeseen JSON format (last token {0}: {1})", jsonReader.TokenType, jsonReader.Value)
                );
            }
        }

        /// <summary>
        /// Advances to the next JSON token and throws if no other token is available.
        /// </summary>
        protected static void ForcedRead(JsonTextReader jsonReader) {
            if (!jsonReader.Read())
                throw new JsonSerializationException("Cannot read JSON token from file");
        }

        /// <summary>
        /// Advances the reader into a JSON object and stops at the value of a given property.
        /// Will throw if no object is found or if the property is not present.
        /// </summary>
        protected static void AdvanceToObjectProperty(JsonTextReader jsonReader, string propertyName) {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            ForcedRead(jsonReader);

            if (jsonReader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException("JSON data does not contain an object");

            while(jsonReader.Read()) {
                if(jsonReader.TokenType == JsonToken.PropertyName) {
                    if(propertyName.Equals((string)jsonReader.Value, PlatformConstants.InvariantStringComparison)) {
                        //Yep, here we are
                        return;
                    }
                }
                else if(jsonReader.TokenType == JsonToken.EndObject) {
                    //Object token is finished and the property was not found
                    throw new JsonSerializationException(string.Format("JSON object does not contain {0} property", propertyName));
                }

                jsonReader.Skip();
            }

            //Whops, major JSON formatting error
            throw new JsonSerializationException("Malformed JSON");
        }

        protected enum ParsingContinuation {
            Continue,
            Stop
        }

        /// <summary>
        /// Advances the reader into a JSON object and performs property-by-property parsing
        /// using callback handlers.
        /// Will throw if no object is found or if JSON is malformed.
        /// </summary>
        /// <param name="handlers">Handlers for JSON object properties.</param>
        /// <param name="endHandler">Handler called when the JSON object ends.</param>
        protected static void ParseObject(JsonTextReader jsonReader,
            IDictionary<string, Func<JsonTextReader, ParsingContinuation>> handlers,
            Action<JsonTextReader> endHandler = null) {

            if (jsonReader == null)
                throw new ArgumentNullException(nameof(jsonReader));
            if (handlers == null)
                throw new ArgumentNullException(nameof(handlers));

            ForcedRead(jsonReader);

            if (jsonReader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException("JSON data does not contain an object");

            while (jsonReader.Read()) {
                if (jsonReader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)jsonReader.Value;
                    if (handlers.ContainsKey(propertyName)) {
                        //Advance to value
                        ForcedRead(jsonReader);

                        //Evaluate reader here
                        var propertyResult = handlers[propertyName](jsonReader);
                        if(propertyResult == ParsingContinuation.Stop) {
                            break;
                        }
                    }
                }
                else if (jsonReader.TokenType == JsonToken.EndObject) {
                    break;
                }

                jsonReader.Skip();
            }

            if(endHandler != null) {
                endHandler(jsonReader);
            }
        }

        /// <summary>
        /// Fully parses a data package.
        /// </summary>
        /// <remarks>
        /// During parsing the position inside the stream may be moved and will not
        /// be reset on return. The stream will be left open and not disposed.
        /// </remarks>
        public abstract DataPackage Parse(Stream input);

        /// <summary>
        /// Extracts the information header of a data package.
        /// </summary>
        /// <remarks>
        /// During parsing the position inside the stream may be moved and will not
        /// be reset on return. The stream will be left open and not disposed.
        /// </remarks>
        public abstract DataPackageInfo ParseInfo(Stream input);

        /// <summary>
        /// Gets a default JSON serializer.
        /// </summary>
        protected JsonSerializer Json { get; private set; }

    }

}
