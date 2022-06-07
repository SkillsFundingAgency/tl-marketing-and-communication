using System.IO;
using System.Net.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using NSubstitute;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests.Extensions;

    public static class FunctionContextExtensions
    {
        public static HttpRequestData BuildHttpRequestData(
            this FunctionContext functionContext, HttpMethod method = default)
        {
            var request = Substitute.For<HttpRequestData>(functionContext);
            request.Method
                .Returns((method ?? HttpMethod.Get).ToString());

            var responseData = functionContext.BuildHttpResponseData();
            request.CreateResponse()
                .Returns(responseData);

        return request;
        }

        public static HttpResponseData BuildHttpResponseData(
            this FunctionContext functionContext)
        {
            var responseData = Substitute.For<HttpResponseData>(functionContext);

            var responseHeaders = new HttpHeadersCollection();
            responseData.Headers.Returns(responseHeaders);

            var responseBody = new MemoryStream();
            responseData.Body.Returns(responseBody);

            return responseData;
        }
}

