using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartRoadSense.Toolkit {

    /// <summary>
    /// HTTP message handler that writes out a HTTP request's contents.
    /// </summary>
    internal class WriteOutHttpMessageHandler : HttpMessageHandler {

        private readonly Stream _output;

        public WriteOutHttpMessageHandler(Stream output) {
            _output = output;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            var requestContent = await request.Content.ReadAsStreamAsync();

            await requestContent.CopyToAsync(_output);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

    }

}
