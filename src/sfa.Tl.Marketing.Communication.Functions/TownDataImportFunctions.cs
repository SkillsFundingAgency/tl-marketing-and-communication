using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Functions;

public class TownDataImportFunctions
{
    private readonly ITownDataService _townDataService; 
    private readonly ITableStorageService _tableStorageService;

    public TownDataImportFunctions
    (
        ITownDataService townDataService,
        ITableStorageService tableStorageService)
    {
        _townDataService = townDataService ?? throw new ArgumentNullException(nameof(townDataService));
        _tableStorageService = tableStorageService ?? throw new ArgumentNullException(nameof(tableStorageService));
    }

    [Function("TownDataManualImport")]
    public async Task<HttpResponseData> ManualImport(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
        HttpRequestData request,
        FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger("HttpFunction");

        try
        {
            logger.LogInformation("Town data manual import function was called.");

            var resultsCount = await _townDataService.ImportTowns();

            logger.LogInformation("Town data manual saved {resultsCount} towns.", resultsCount);
            logger.LogInformation("Town data manual import finished.");

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/text");

            await response.WriteStringAsync(
                $"Saved {resultsCount} towns.");

            return response;
        }
        catch (Exception e)
        {
            logger.LogError("Error importing town data. Internal Error Message {e}", e);

            return request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
    
    [Function("GetTowns")]
    public async Task<HttpResponseData> GetTowns(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequestData request,
        FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger("HttpFunction");

        try
        {
            logger.LogInformation("GetTowns function was called.");

            var towns =
                (await _tableStorageService.GetAllTowns())
                .OrderBy(t => t.Name)
                .ToList();

            logger.LogInformation("GetTowns returned {townsCount} records.", towns.Count);

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            var json = JsonSerializer.Serialize(towns,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            await response.WriteStringAsync(json);

            return response;
        }
        catch (Exception e)
        {
            logger.LogError("Error in GetTowns. Internal Error Message {e}", e);

            return request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}