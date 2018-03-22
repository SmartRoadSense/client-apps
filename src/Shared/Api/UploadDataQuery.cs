using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SmartRoadSense.Shared.DataModel;

namespace SmartRoadSense.Shared.Api {

    /// <summary>
    /// Performs an anonymous upload of a track.
    /// </summary>
    public class UploadDataQuery : BaseQuery<UploadDataQuery.UploadDataQueryResult> {
        
        public class UploadDataQueryResult {

            public UploadDataQueryResult(int uploadedTrackId) {
                UploadedTrackId = uploadedTrackId;
            }

            public readonly int UploadedTrackId;

        }

        public UploadDataQuery () 
            : base("/data") {

        }

        #region Parameters

        private byte[] _secretHash = null;

        /// <summary>
        /// Gets or sets the track's secret hash.
        /// Must be <see cref="Crypto.SecretHashLength"/> bytes long.
        /// </summary>
        public byte[] SecretHash {
            get {
                return _secretHash;
            }
            set {
                if (value == null || value.Length != Crypto.SecretHashLength) {
                    throw new ArgumentException("Secret hash null or invalid");
                }

                _secretHash = value;
            }
        }

        /// <summary>
        /// Gets or sets the track to be uploaded.
        /// </summary>
        public DataPackage Package { get; set; }

        private void CheckQueryArguments() {
            if (Package == null)
                throw new ArgumentException("Data not set");

            if (Package.Pieces == null || Package.Pieces.Count < 1)
                throw new ArgumentException("Cannot upload an empty set of data");

            if (SecretHash == null || SecretHash.Length != Crypto.SecretHashLength)
                throw new ArgumentException("Secret hash not set or invalid");
        }

        #endregion

        protected override HttpContent CreateRequestContent() {
            CheckQueryArguments();

            return new UploadDataHttpContent(this);
        }

        protected override HttpMethod Method {
            get {
                return HttpMethod.Post;
            }
        }

        protected override void PrepareRequest(HttpRequestMessage request) {
            base.PrepareRequest(request);

            request.Headers.Accept.Set(new MediaTypeWithQualityHeaderValue(ApplicationJsonMimeType));
        }

        protected override UploadDataQueryResult ParseJsonResponse(HttpResponseMessage response, JObject jobj) {
            var uploadedId = jobj.Value<int?>("track");
            if(!uploadedId.HasValue)
                throw new ProtocolViolationException("Response does not contain an uploaded track ID");

            return new UploadDataQueryResult(uploadedId.Value);
        }

        /// <summary>
        /// Internal HTTP content class for <see cref="UploadDataQuery"/>.
        /// </summary>
        protected class UploadDataHttpContent : JsonHttpContent {

            private readonly UploadDataQuery _query;

            public UploadDataHttpContent(UploadDataQuery query) {
                _query = query;
            }

            protected override void SerializeJson(JsonTextWriter writer, TransportContext context) {
                var serializer = JsonSerializer.Create(App.JsonSettings);
                
                Log.Debug("Creating JSON payload for {0} data points", _query.Package.Pieces.Count);
                var payloadItems = from p in _query.Package.Pieces
                                   select UploadPayload.Create(p);
                var jsonPayloadItems = serializer.SerializeToString(payloadItems);

                var lastPiece = _query.Package.Pieces.Last();
                var anchorage = lastPiece.Anchorage;
                var vehicle = lastPiece.Vehicle;
                var numberOfPeople = lastPiece.NumberOfPeople;

                var metadata = UploadMetadata.Create();
                metadata.NumberOfPeople = numberOfPeople;
                var jsonMetadata = serializer.SerializeToString(metadata);

                //JSON payload output
                writer.WriteStartObject();

                writer.WritePropertyName("secret");
                writer.WriteValue(_query.SecretHash.ToBase64());

                writer.WritePropertyName("metadata");
                writer.WriteValue(jsonMetadata);

                writer.WritePropertyName("anchorage-type");
                writer.WriteValue(anchorage);

                writer.WritePropertyName("vehicle-type");
                writer.WriteValue(vehicle);

                writer.WritePropertyName("number-of-people");
                writer.WriteValue(numberOfPeople);

                writer.WritePropertyName("payload");
                writer.WriteValue(jsonPayloadItems);

                writer.WritePropertyName("payload-hash");
                writer.WriteValue(jsonPayloadItems.ToSha512Hash().ToBase64());

                writer.WritePropertyName("time");
                writer.WriteValue(DateTime.UtcNow);

                writer.WriteEndObject();
            }

            /// <summary>
            /// Internal POCO class used to serialize data points to JSON.
            /// </summary>
            [JsonObject(MemberSerialization.OptIn)]
            #if __ANDROID__
            [global::Android.Runtime.Preserve(AllMembers = true)]
            #endif
            private class UploadPayload {

                public UploadPayload() {
                }

                [JsonProperty("bearing")]
                public double Bearing { get; set; }

                [JsonProperty("time")]
                public DateTime Time { get; set; }

                [JsonProperty("duration")]
                public int Duration { get; set; }

                [JsonProperty("position-resolution")]
                public int PositionResolution { get; set; }

                [JsonProperty("position")]
                public Point Position { get; set; }

                [JsonProperty("values")]
                public double[] Values { get; set; }

                public static UploadPayload Create(Data.DataPiece piece) {
                    var pos = new Position(piece.Latitude, piece.Longitude);
                    return new UploadPayload {
                        Bearing = piece.Bearing,
                        Time = piece.StartTimestamp,
                        Duration = (int)(piece.EndTimestamp - piece.StartTimestamp).TotalMilliseconds,
                        PositionResolution = piece.Accuracy,
                        Position = new Point(pos),
                        Values = new double[] { piece.PpeX, piece.PpeY, piece.PpeZ, piece.Ppe, piece.Speed }
                    };
                }

            }

            /// <summary>
            /// Internal POCO class used to serialize metadata to JSON.
            /// </summary>
            [JsonObject(MemberSerialization.OptIn)]
            #if __ANDROID__
            [global::Android.Runtime.Preserve(AllMembers = true)]
            #endif
            private class UploadMetadata {

                public UploadMetadata() {
                }

                public Version ApplicationVersion { get; set; }

                [JsonProperty("appVersion")]
                public string ApplicationVersionString {
                    get {
                        return ApplicationVersion.ToString();
                    }
                }

                [JsonProperty("operatingSystem")]
                public string OperatingSystem { get; set; }

                public Version OperatingSystemVersion { get; set; }

                [JsonProperty("operatingSystemVersion")]
                public string OperatingSystemVersionString {
                    get {
                        return OperatingSystemVersion.ToString();
                    }
                }

                [JsonProperty("sdk")]
                public string Sdk { get; set; }

                [JsonProperty("deviceManufacturer")]
                public string DeviceManufacturer { get; set; }

                [JsonProperty("deviceModel")]
                public string DeviceModel { get; set; }

                [JsonProperty("accelerometerScaleFactor")]
                public double AccelerometerScaleFactor { get; set; }

                [JsonProperty("numberOfPeople")]
                public int NumberOfPeople { get; set; }

                public static UploadMetadata Create() {
                    var deviceInfo = DeviceInformation.Current;

                    return new UploadMetadata {
                        ApplicationVersion = App.Version,
                        OperatingSystem = deviceInfo.OperatingSystemName,
                        OperatingSystemVersion = deviceInfo.OperatingSystemVersion,
                        Sdk = deviceInfo.SdkVersion,
                        DeviceManufacturer = deviceInfo.Manufacturer,
                        DeviceModel = deviceInfo.Model,
                        AccelerometerScaleFactor = Settings.CalibrationScaleFactor
                    };
                }

            }

        }

    }

}
