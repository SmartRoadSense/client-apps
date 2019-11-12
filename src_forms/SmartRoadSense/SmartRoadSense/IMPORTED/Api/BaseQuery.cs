using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmartRoadSense {

    /// <summary>
    /// Base SmartRoadSense API query.
    /// </summary>
    public abstract class BaseQuery<T> {

        public const string ApplicationJsonMimeType = "application/json";

        protected BaseQuery(string path) {
            if (path == null)
                throw new ArgumentNullException();

            _path = path;

            Compression = CompressionPolicy.Default;

#if DEBUG
            Service = ServiceType.Development;
#else
            Service = ServiceType.Production;
#endif
        }

        private readonly string _path;

        /// <summary>
        /// Gets the query's base URI.
        /// </summary>
        private string BaseUri {
            get {
                switch(Service) {
                    case ServiceType.Production:
                        return "http://api.smartroadsense.it/api";

                    case ServiceType.Development:
                        return "http://smartroadsense.uniurb.it/api";

                    default:
                        throw new ArgumentException(string.Format("Unsupported service type ({0})", Service));
                }
            }
        }

        /// <summary>
        /// Gets the final URL used by the API query.
        /// </summary>
        public virtual string Url {
            get {
                return string.Concat(BaseUri, _path);
            }
        }

        private static HttpClient _http = null;

        /// <summary>
        /// Gets the shared HTTP client.
        /// </summary>
        protected HttpClient Http {
            get {
                if(OverrideHttpClient != null) {
                    return OverrideHttpClient;
                }

                if (_http == null) {
                    Log.Debug("Initializing default HTTP client");
                    _http = new HttpClient();
                }

                return _http;
            }
        }

        /// <summary>
        /// Gets or sets the custom HTTP client that will be used to perform the
        /// API request instead of the default client.
        /// </summary>
        public HttpClient OverrideHttpClient { get; set; }

        /// <summary>
        /// Gets the HTTP method used by the API query.
        /// </summary>
        protected virtual HttpMethod Method {
            get {
                return HttpMethod.Get;
            }
        }

        /// <summary>
        /// Gets or sets the compression policy of the query.
        /// </summary>
        public CompressionPolicy Compression { get; set; }

        /// <summary>
        /// Gets or sets the type of API service to use.
        /// </summary>
        public ServiceType Service { get; set; }

        /// <summary>
        /// Executes the query. Will throw on error.
        /// </summary>
        /// <exception cref="System.Net.WebException">Generic HTTP access error.</exception>
        /// <exception cref="Systen.Net.ProtocolViolationException">SmartRoadSense API protocol violation.</exception>
        public async Task<T> Execute(CancellationToken cancellationToken) {
            var url = Url;

            Log.Debug("Initiating {0} HTTP request to {1}", Method, url);
            var request = await Task.Run(() => {
                var ret = new HttpRequestMessage(Method, url);
                ret.Content = CreateRequestContent();
                PrepareRequest(ret);

                return ret;
            });

            //Execution
            Log.Debug("Executing {0} HTTP request to {1}", Method, url);
            var response = await Http.SendAsync(request, cancellationToken);

            //Response parsing
            Log.Debug("Processing {0} HTTP response from {1} ({2})", Method, url, response.StatusCode);
            return await ProcessResponse(response);
        }

        /// <summary>
        /// Prepares the HTTP request and sets headers as appropriate.
        /// </summary>
        protected virtual void PrepareRequest(HttpRequestMessage request) {
            //Add compression, if possible
            if(request.Content != null &&
                (Compression & CompressionPolicy.RequestGZip) == CompressionPolicy.RequestGZip) {
                request.Content = new GZipHttpContent(request.Content);
            }

            //Request response compression
            if((Compression & CompressionPolicy.AcceptGzip) == CompressionPolicy.AcceptGzip) {
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            }
        }

        /// <summary>
        /// Creates the new content for the web request.
        /// </summary>
        /// <remarks>
        /// The HttpContent is passed to the HttpClient and disposed after the request
        /// has been executed.
        /// </remarks>
        protected virtual HttpContent CreateRequestContent() {
            return null;
        }

        protected async Task<T> ProcessResponse(HttpResponseMessage response) {
            response.EnsureSuccessStatusCode();

            if (response.Content == null)
                return default(T);

            //TODO handle incoming compression
            var responseContents = await response.Content.ReadAsStringAsync();

#if DEBUG
            Log.Debug("Response: {0}", responseContents);
#endif

            return await Task<T>.Run(() => {
                var jobj = JObject.Parse(responseContents);

                if (response.IsSuccessStatusCode) {
                    var responseResult = jobj.Value<string>("result");

                    //Common response fields
                    if (!"OK".Equals(responseResult, PlatformConstants.InvariantStringComparison)) {
                        throw new ProtocolViolationException(string.Format("Successful API request returned status {0}", responseResult));
                    }

                    //Specific response parsing
                    return ParseJsonResponse(response, jobj);
                }
                else {
                    //Common server error handling
                    var code = (int?)jobj["code"];
                    string errorMsg = (string)jobj["message"];

                    throw new ProtocolViolationException(string.Format("Failed API request (code {0}, {1})", code, errorMsg));
                }
            });
        }

        protected abstract T ParseJsonResponse(HttpResponseMessage response, JObject jobj);

        /// <summary>
        /// Asynchronously generates HTTP content in JSON format.
        /// </summary>
        /// <param name="writer">JSON writer used to compose the HTTP content.</param>
        protected Task<HttpContent> CreateJsonContent(Action<JsonWriter> writer) {
            return Task<HttpContent>.Run(() => {
                //TODO: the recyclable high performance memory stream might be used here
                var memoryStream = new MemoryStream();

                try {
                    //NOTE: memory stream must not be disposed before passing on to StreamContent,
                    //      because it must be left open for HTTP transmission.
                    //      StreamWriter does not close underlying stream when finalized, thus
                    //      is left to the GC.
                    //      JsonWriter does always follow its CloseOutput property, thus can be
                    //      disposed after usage (this does not flush the stream however).
                    var streamWriter = new StreamWriter(memoryStream);

                    using (var jsonWriter = new JsonTextWriter(streamWriter)) {
                        jsonWriter.CloseOutput = false;
                        writer(jsonWriter);
                        jsonWriter.Flush();
                    }

                    streamWriter.Flush();

                    //Rewind memory stream
                    memoryStream.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception ex) {
                    Log.Error(ex, "Failed to create request content");

                    memoryStream.Dispose();
                    throw;
                }

                var content = new StreamContent(memoryStream);
                content.Headers.ContentType = new MediaTypeHeaderValue(ApplicationJsonMimeType);

                return (HttpContent)content;
            });
        }

    }

}
