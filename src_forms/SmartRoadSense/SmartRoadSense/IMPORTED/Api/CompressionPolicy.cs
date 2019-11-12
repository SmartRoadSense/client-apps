using System;

namespace SmartRoadSense {

    [Flags]
    public enum CompressionPolicy {
        /// <summary>
        /// Compression disabled for requests and responses.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// HTTP requests are sent using GZip compression.
        /// </summary>
        RequestGZip = 1,

        /// <summary>
        /// Accepts GZip compression in HTTP responses.
        /// </summary>
        AcceptGzip = 1 << 16,
        /// <summary>
        /// Accepts all supported compression schemes in HTTP responses.
        /// </summary>
        AcceptAll = AcceptGzip,

        Default = RequestGZip | AcceptAll
    }

}
