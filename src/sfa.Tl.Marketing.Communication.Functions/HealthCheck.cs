using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Functions;

public class HealthCheck
{
    private readonly HealthCheckService _healthCheck;

    public HealthCheck(HealthCheckService healthCheck)
    {
        _healthCheck = healthCheck;
    }

    [Function("HealthCheck")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
    {
        try
        {
            HealthReport report = await _healthCheck.CheckHealthAsync();

            return report.Status == HealthStatus.Healthy
                ? new OkObjectResult(report)
                : new ObjectResult(report) { StatusCode = StatusCodes.Status500InternalServerError };
        }
        catch (Exception ex)
        {
            return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}