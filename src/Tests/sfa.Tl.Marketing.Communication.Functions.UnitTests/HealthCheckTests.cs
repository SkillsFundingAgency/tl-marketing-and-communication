using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using sfa.Tl.Marketing.Communication.Functions.UnitTests.Builders;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests;

public class HealthCheckTests
{
    [Theory]
    [InlineData(true, HttpStatusCode.OK)]
    [InlineData(false, HttpStatusCode.InternalServerError)]
    public async Task HealthCheck_Returns_Status_Of_Registered_Check(bool healthy, HttpStatusCode expectedStatus)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHealthChecks().AddCheck("probe", () =>
            healthy ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy());

        using var provider = services.BuildServiceProvider();
        var healthCheck = new HealthCheck(provider.GetRequiredService<HealthCheckService>());
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Get);

        var response = await healthCheck.Run(request);

        response.StatusCode.Should().Be(expectedStatus);
        response.Headers.GetValues("Content-Type").Should().Contain("application/json");
    }
}
