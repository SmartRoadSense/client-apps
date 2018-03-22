using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared.Api {

    /// <summary>
    /// HTTP content that asynchronously serializes its contents and set HTTP headers.
    /// </summary>
    public abstract class JsonHttpContent : HttpContent {

        public JsonHttpContent() {
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) {
            return Task.Run(() => {
                var writer = new StreamWriter(stream);

                using (var jsonWriter = new JsonTextWriter(writer)) {
                    jsonWriter.CloseOutput = false;

                    SerializeJson(jsonWriter, context);
                }

                writer.Flush();
            });
        }

        protected abstract void SerializeJson(JsonTextWriter writer, TransportContext context);

        protected override bool TryComputeLength(out long length) {
            //Cannot guess serialization result
            length = -1;
            return false;
        }

    }

}
