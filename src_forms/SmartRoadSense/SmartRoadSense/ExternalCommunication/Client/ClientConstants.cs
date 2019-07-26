using System;
namespace SmartRoadSense
{
    public class ClientConstants
    {
        public struct BaseEndpoint
        {
            // VERSION CALLS
            readonly static public string BaseApiUrl = "srs-api-url";
            readonly static public string V1 = "/v1";
        }

        public struct Configuration
        {
            readonly static public long DefaultBufferSize = 4096 * 1000;
        }

        public struct Track
        {
            public struct HeaderKey
            {
                readonly static public string Authorization = "Authorization";
                readonly static public string AuthorizationPrefix = "Bearer ";
            }

            public struct Resource
            {
                readonly static public string ResourcePath = BaseEndpoint.BaseApiUrl + BaseEndpoint.V1 + "/endpoint";

                public struct StatusCode
                {
                    public const int Ok = 200;
                    public const int Malformed = 400;
                    public const int Unauthorized = 401;
                }
            }
        }
    }
}
