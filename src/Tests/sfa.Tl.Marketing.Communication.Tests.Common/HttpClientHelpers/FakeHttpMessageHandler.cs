using System.Net;

namespace sfa.Tl.Marketing.Communication.Tests.Common.HttpClientHelpers;

public class FakeHttpMessageHandler : DelegatingHandler
{
    private readonly Dictionary<Uri, HttpResponseMessage> _fakeResponses = new();

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