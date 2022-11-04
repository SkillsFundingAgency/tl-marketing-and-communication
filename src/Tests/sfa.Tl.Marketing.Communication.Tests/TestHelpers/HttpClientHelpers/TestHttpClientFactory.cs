using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace sfa.Tl.Marketing.Communication.UnitTests.TestHelpers.HttpClientHelpers;

public class TestHttpClientFactory
{
    public HttpClient CreateHttpClientWithBaseUri(Uri baseUri, string relativeUri, string response, string responseContentType = "application/json", HttpStatusCode responseCode = HttpStatusCode.OK)
    {
        var httpResponseMessage = CreateFakeResponse(response, responseContentType, responseCode);

        var uri = new Uri(baseUri, relativeUri);
        var fakeMessageHandler = new FakeHttpMessageHandler();
        fakeMessageHandler.AddFakeResponse(uri, httpResponseMessage);

        var httpClient = new HttpClient(fakeMessageHandler)
        {
            BaseAddress = baseUri
        };

        return httpClient;
    }

    public HttpResponseMessage CreateFakeResponse(string response, string responseContentType = "application/json", HttpStatusCode responseCode = HttpStatusCode.OK)
    {
        var httpResponseMessage = new HttpResponseMessage(responseCode)
        {
            Content = new StringContent(response)
        };

        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(responseContentType);

        return httpResponseMessage;
    }

    public HttpResponseMessage CreateFakeResponse(Stream response, string responseContentType = "application/json", HttpStatusCode responseCode = HttpStatusCode.OK)
    {
        var httpResponseMessage = new HttpResponseMessage(responseCode)
        {
            Content = new StreamContent(response)
        };
        
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(responseContentType);

        return httpResponseMessage;
    }

    public HttpClient CreateClient(Uri baseUri, IEnumerable<(string relativeUri, string json, HttpStatusCode statusCode)> responses, string contentType = "application/json")
    {
        var fakeMessageHandler = new FakeHttpMessageHandler();

        foreach (var (relativeUri, json, statusCode) in responses)
        {
            var httpResponseMessage = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json)
            };
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            var uri = new Uri(baseUri, relativeUri);
            fakeMessageHandler.AddFakeResponse(uri, httpResponseMessage);
        }

        return new HttpClient(fakeMessageHandler)
        {
            BaseAddress = baseUri
        };
    }
}