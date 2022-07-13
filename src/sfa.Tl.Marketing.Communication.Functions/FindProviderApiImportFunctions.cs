using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Functions;

public class FindProviderApiImportFunctions
{
    private readonly IFindProviderApiDataService _findProviderApiDataService;

    public FindProviderApiImportFunctions(
        IFindProviderApiDataService findProviderApiDataService)
    {
        _findProviderApiDataService = findProviderApiDataService ?? throw new ArgumentNullException(nameof(findProviderApiDataService));
    }

    [Function("FindProviderApiScheduledImport")]
    public async Task ImportFindProviderApiData(
        [TimerTrigger("%FindProviderApiImportTrigger%"
        #if DEBUG
                    //Fixes problem with functions startup from VS2019
                    // - see https://github.com/Azure/azure-functions-dotnet-worker/issues/471
                    , UseMonitor = false
        #endif
                )]
                // ReSharper disable once UnusedParameter.Global
                TimerInfo timer,
        FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger("TimerFunction");

        try
        {
            logger.LogInformation("Find provider scheduled import function was called.");

            var (savedQualifications, deletedQualifications) = await _findProviderApiDataService.ImportQualificationsFromFindProviderApi();
            logger.LogInformation("Find a provider import inserted or updated {savedQualifications} and deleted {deletedQualifications} qualifications.", savedQualifications, deletedQualifications);

            var (savedProviders, deletedProviders) =
                await _findProviderApiDataService.ImportProvidersFromFindProviderApi();
            logger.LogInformation("Find a provider import inserted or updated {savedProviders} and deleted {deletedProviders} providers.", savedProviders, deletedProviders);

            logger.LogInformation("Find a provider scheduled import finished.");
        }
        catch (Exception e)
        {
            logger.LogError("Error importing data from find a provider API. Internal Error Message {e}", e);
        }
    }

    [Function("FindProviderApiManualImport")]
    public async Task<HttpResponseData> ManualImport(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
        HttpRequestData request,
        FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger("HttpFunction");

        try
        {
            logger.LogInformation("Find provider manual import function was called.");

            var (savedQualifications, deletedQualifications) = await _findProviderApiDataService.ImportQualificationsFromFindProviderApi();

            var (savedProviders, deletedProviders) =
                await _findProviderApiDataService.ImportProvidersFromFindProviderApi();
            logger.LogInformation("Find a provider import inserted or updated {savedProviders} and deleted {deletedProviders} providers.", savedProviders, deletedProviders);
           
            logger.LogInformation("Find provider manual import finished.");

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/text");

            await response.WriteStringAsync(
                $"Inserted or updated {savedProviders} and deleted {deletedProviders} providers.\r\n" +
                $"Inserted or updated {savedQualifications} and deleted {deletedQualifications} qualifications.");

            return response;
        }
        catch (Exception e)
        {
            logger.LogError("Error importing data from find provider API. Internal Error Message {e}", e);

            return request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    [Function("GetFindProviderApiProvidersJson")]
    public async Task<HttpResponseData> GetFindProviderApiProvidersJson(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequestData request,
        FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger("HttpFunction");

        try
        {
            logger.LogInformation("Find a Provider GetFindProviderApiProvidersJson function was called.");

            var json = await _findProviderApiDataService.GetProvidersJsonFromFindProviderApi();

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(json);

            return response;
        }
        catch (Exception e)
        {
            logger.LogError("Error reading json data from find provider API. Internal Error Message {e}", e);

            return request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    [Function("GetFindProviderApiQualificationsJson")]
    public async Task<HttpResponseData> GetFindProviderApiQualificationsJson(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequestData request,
        FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger("HttpFunction");
        try
        {
            logger.LogInformation("Find a Provider GetFindProviderApiQualificationsJson function was called.");

            var json = await _findProviderApiDataService.GetQualificationsJsonFromFindProviderApi();

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(json);

            return response;
        }
        catch (Exception e)
        {
            logger.LogError("Error reading json data from find provider API. Internal Error Message {e}", e);

            return request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}