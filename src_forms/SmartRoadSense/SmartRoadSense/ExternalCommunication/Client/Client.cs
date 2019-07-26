using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartRoadSense
{
    public class Client : IClient
    {
        readonly static string baseUrl = ClientConstants.BaseEndpoint.BaseApiUrl;
        readonly static long bufferSize = ClientConstants.Configuration.DefaultBufferSize;

        HttpClient _client;
        readonly Uri _uri;
        HttpRequestMessage _request;

        public Client()
        {
            _client = new HttpClient()
            {
                MaxResponseContentBufferSize = bufferSize
            };

            _uri = new Uri(baseUrl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="urlResource"></param>
        /// <param name="headerParams"></param>
        public void PrepareRequest(HttpMethod method, string urlResource, Dictionary<string, string> headerParams)
        {
            var endpoint = _uri + urlResource;
            Debug.WriteLine("resource called:" + endpoint);

            var requestMessage = new HttpRequestMessage(method, endpoint);
            if (headerParams != null && headerParams.Count > 0)
                foreach (KeyValuePair<string, string> entry in headerParams)
                {
                    requestMessage.Headers.Add(entry.Key, entry.Value);
                }

            _request = requestMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="urlResource"></param>
        /// <param name="headerParams"></param>
        /// <param name="body"></param>
        public void PrepareRequest(HttpMethod method, string urlResource, Dictionary<string, string> headerParams, List<Object> body)
        {
            var endpoint = _uri + urlResource;

            var requestMessage = new HttpRequestMessage(method, endpoint);
            if (headerParams != null && headerParams.Count > 0)
            {
                foreach (KeyValuePair<string, string> entry in headerParams)
                {
                    requestMessage.Headers.Add(entry.Key, entry.Value);
                }
            }
            var param = JsonConvert.SerializeObject(body);
            HttpContent contentPost = new StringContent(param, Encoding.UTF8, "application/json");
            requestMessage.Content = contentPost;
            _request = requestMessage;
        }

        /// <summary>
        /// Send RESTful request with headers and json body
        /// </summary>
        /// <param name="method">Method.</param>
        /// <param name="urlResource">URL resource.</param>
        /// <param name="headerParams">Header parameters.</param>
        /// <param name="body">Body.</param>
        public void PrepareRequest(HttpMethod method, string urlResource, Dictionary<string, string> headerParams, object body)
        {
            var endpoint = _uri + urlResource;
            Debug.WriteLine("resource called:" + endpoint);

            var requestMessage = new HttpRequestMessage(method, endpoint);
            if (headerParams != null)
                foreach (KeyValuePair<string, string> entry in headerParams)
                {
                    requestMessage.Headers.Add(entry.Key, entry.Value);
                }

            var param = JsonConvert.SerializeObject(body);
            HttpContent contentPost = new StringContent(param, Encoding.UTF8, "application/json");
            requestMessage.Content = contentPost;
            _request = requestMessage;
        }


        /// <summary>
        /// Prepare request 
        /// Payload will not been serialized with JsonConvert.SerializeObject
        /// </summary>
        /// <param name="method"></param>
        /// <param name="urlResource"></param>
        /// <param name="headerParams"></param>
        /// <param name="payload"></param>
        public void PrepareRequest(HttpMethod method, string urlResource, Dictionary<string, string> headerParams, Dictionary<string, string> payload)
        {
            var endpoint = _uri + urlResource;
            var requestMessage = new HttpRequestMessage(method, endpoint);
            if (headerParams != null)
                foreach (KeyValuePair<string, string> entry in headerParams)
                {
                    requestMessage.Headers.Add(entry.Key, entry.Value);
                }

            var contentPost = new System.Net.Http.FormUrlEncodedContent(payload);
            requestMessage.Content = contentPost;
            _request = requestMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> SendRequest()
        {
            return _client.SendAsync(_request, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postVars"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> PostRequest(Dictionary<string, string> postVars)
        {
            var content = new System.Net.Http.FormUrlEncodedContent(postVars);
            return _client.PostAsync(_request.RequestUri, content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> SendLargeContentRequest()
        {
            return _client.SendAsync(_request, HttpCompletionOption.ResponseHeadersRead);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Stream> GetStreamAsync()
        {
            return _client.GetStreamAsync(_uri);
        }
    }
}
