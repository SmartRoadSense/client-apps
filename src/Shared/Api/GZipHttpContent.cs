using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace SmartRoadSense.Shared.Api {

    /// <summary>
    /// HttpContent implementation that wraps another HttpContent value and
    /// performs on-the-fly GZip compression.
    /// HTTP headers are set according to HTTP spec.
    /// </summary>
    internal class GZipHttpContent : HttpContent {

        private readonly HttpContent _originalContent;

        public GZipHttpContent(HttpContent originalContent) {
            if (originalContent == null)
                throw new ArgumentNullException("originalContent");
            
            _originalContent = originalContent;

            foreach (var header in originalContent.Headers) {
                Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            //NOTE: since the content is encoded using GZip "on-the-fly", its encoding
            //      header is set here.
            //      For transport-level "on-the-fly" compression (which shouldn't impact
            //      the content's encoding), the "transfer-encoding" header should be
            //      used at a higher level.
            //      See: http://stackoverflow.com/q/11641923/3118
            Headers.ContentEncoding.Set("gzip");
        }

        public HttpContent OriginalContent {
            get {
                return _originalContent;
            }
        }

        #region Implemented abstract members of HttpContent

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) {
            //TODO: check whether this "leaveOpen" is needed
            var compressedStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true);

            return _originalContent.CopyToAsync(compressedStream, context).ContinueWith(tsk => {
                if(compressedStream != null) {
                    compressedStream.Dispose();
                }
            });
        }

        protected override bool TryComputeLength(out long length) {
            //No idea how to predict compressed length
            length = -1;
            return false;
        }

        #endregion

    }

}
