using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application
{
    public class FakeHttpMessageHandler : DelegatingHandler
    {
        private readonly Dictionary<Uri, HttpResponseMessage> _fakeResponses 
            // ReSharper disable once ArrangeObjectCreationWhenTypeEvident
#pragma warning disable IDE0090 // Use 'new(...)'
            = new Dictionary<Uri, HttpResponseMessage>();
#pragma warning restore IDE0090 // Use 'new(...)'

        public void AddFakeResponse(Uri uri, HttpResponseMessage responseMessage)
        {
            _fakeResponses.Add(uri, responseMessage);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri != null && 
                _fakeResponses.ContainsKey(request.RequestUri))
            {
                return Task.FromResult(_fakeResponses[request.RequestUri]);
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                RequestMessage = request
            });
        }
    }
}