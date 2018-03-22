using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json.Bson;

namespace ProtocolPayloadBenchmark {

    class Program {

        static void Main(string[] args) {
            var sizes = new int[] {
                10, 100, 500, 1000, 5000, 10000, 20000, 30000, 50000
            };

            foreach(var size in sizes) {
                //PerformPayloadBenchmarks(size);
                PerformDiskSerializationBenchmarks(size);
            }

            Console.Read();
        }

        private static void PerformDiskSerializationBenchmarks(int size) {
            var dataElements = (from i in Enumerable.Range(0, size)
                                select DataPiece.GenerateRandom()).ToArray();

            var stream = new MemoryStream();
            using(stream) {
                using(var writer = new JsonTextWriter(new StreamWriter(stream))) {
                    var serializer = JsonSerializer.Create();
                    serializer.Serialize(writer, dataElements, typeof(ICollection<DataPiece>));
                }
            }

            Console.WriteLine("Serialized file: {0} -> {1}", size, stream.ToArray().Length);
        }

        private static readonly UploadMetadata Metadata = new UploadMetadata {
            ApplicationVersion = new Version(1, 0, 0, 0),
            OperatingSystem = "Whatever OS",
            OperatingSystemVersion = new Version(1, 0, 0, 0),
            Sdk = "11",
            DeviceManufacturer = "Contoso",
            DeviceModel = "Whateverphone"
        };

        private static void PerformPayloadBenchmarks(int size) {
            var payloadElements = (from i in Enumerable.Range(0, size)
                                   select UploadPayload.GenerateRandom()).ToArray();

            PerformSingleBenchmark(payloadElements, s => {
                return new JsonTextWriter(new StreamWriter(s));
            }, "JSON");

            PerformSingleBenchmark(payloadElements, s => {
                return new JsonTextWriter(new StreamWriter(new GZipStream(s, CompressionLevel.Fastest)));
            }, "JSON + GZip (fast)");

            PerformSingleBenchmark(payloadElements, s => {
                return new JsonTextWriter(new StreamWriter(new GZipStream(s, CompressionLevel.Optimal)));
            }, "JSON + GZip (optimal)");
        }

        private static void PerformSingleBenchmark(UploadPayload[] data, Func<Stream, JsonWriter> streamer, string title) {
            var watch = new Stopwatch();

            var stream = new MemoryStream();
            using (stream) {
                watch.Start();

                var jsonData = data.ToJson();

                using (var writer = streamer(stream)) {
                    writer.CloseOutput = true;
                     
                    writer.WriteStartObject();
                    writer.WritePropertyName("track-id");
                    writer.WriteValue(Guid.NewGuid().ToByteArray().ToShaHash().ToBase64());
                    writer.WritePropertyName("metadata");
                    writer.WriteValue(Metadata.ToJson());
                    writer.WritePropertyName("anchorage-type");
                    writer.WriteValue(1);
                    writer.WritePropertyName("vehicle-type");
                    writer.WriteValue(1);
                    writer.WritePropertyName("payload");
                    writer.WriteValue(jsonData);
                    writer.WritePropertyName("payload-hash");
                    writer.WriteValue(jsonData.ToSha3Hash().ToBase64());
                    writer.WritePropertyName("time");
                    writer.WriteValue(DateTime.UtcNow);
                    writer.WriteEndObject();

                    writer.Close();
                }

                watch.Stop();
            }

            Console.WriteLine("{0},{1},{2},{3}", title, data.Length, stream.ToArray().Length, watch.ElapsedMilliseconds);
        }
    }
}
