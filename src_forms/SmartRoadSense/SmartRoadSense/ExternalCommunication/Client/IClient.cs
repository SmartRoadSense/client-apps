using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartRoadSense
{
    public interface IClient
    {
        void PrepareRequest(HttpMethod method, string urlResource, Dictionary<string, string> headerParams);

        void PrepareRequest(HttpMethod method, string urlResource, Dictionary<string, string> headerParams, Dictionary<string, string> body);

        void PrepareRequest(HttpMethod method, string urlResource, Dictionary<string, string> headerParams, List<Object> body);

        void PrepareRequest(HttpMethod method, string urlResource, Dictionary<string, string> headerParams, object body);

        Task<HttpResponseMessage> SendRequest();
    }
}
