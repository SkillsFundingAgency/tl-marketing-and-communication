using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace sfa.Tl.Marketing.Communication.Application.Extensions;

public static class HttpClientBuilderExtensions
{
    public static IHttpClientBuilder AddRetryPolicyHandler<T>(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder
            .AddPolicyHandler((serviceProvider, _) =>
                HttpPolicyExtensions.HandleTransientHttpError()
                    .WaitAndRetryAsync(new[]
                        {
                            TimeSpan.FromMilliseconds(200),
                            TimeSpan.FromSeconds(1),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(10),
                            TimeSpan.FromSeconds(30),
                        },
                        (_, timespan, retryAttempt, _) =>
                        {
                            serviceProvider
                                .GetService<ILogger<T>>()?
                                .LogWarning("Transient HTTP error in {Name}. " +
                                            "Delaying for {delayTime}ms, then making retry {retryAttempt}.",
                                    typeof(T).Name, timespan.TotalMilliseconds, retryAttempt);
                        }
                    ));
    }
}