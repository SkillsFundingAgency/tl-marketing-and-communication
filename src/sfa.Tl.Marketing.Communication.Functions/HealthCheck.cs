using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Net;
using System.Text.Json;
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
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req)
    {
        HttpResponseData response = req.CreateResponse();
        response.Headers.Add("Content-Type", "application/json");
        try
        {
            HealthReport report = await _healthCheck.CheckHealthAsync();
            response.StatusCode = report.Status == HealthStatus.Healthy
                ? HttpStatusCode.OK
                : HttpStatusCode.InternalServerError;
            await response.WriteStringAsync(JsonSerializer.Serialize(report));
        }
        catch (Exception ex)
        {
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync(JsonSerializer.Serialize(ex.Message));
        }
        return response;
    }
}