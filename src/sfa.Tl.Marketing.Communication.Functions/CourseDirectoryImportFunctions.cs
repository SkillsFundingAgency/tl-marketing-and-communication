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

public class CourseDirectoryImportFunctions
{
    private readonly ICourseDirectoryDataService _courseDirectoryDataService;
    private readonly ITableStorageService _tableStorageService;

    public CourseDirectoryImportFunctions(
        ICourseDirectoryDataService courseDirectoryDataService,
        ITableStorageService tableStorageService)
    {
        _courseDirectoryDataService = courseDirectoryDataService ?? throw new ArgumentNullException(nameof(courseDirectoryDataService));
        _tableStorageService = tableStorageService ?? throw new ArgumentNullException(nameof(tableStorageService));
    }

    [Function("CourseDirectoryScheduledImport")]
    public async Task ImportCourseDirectoryData(
        [TimerTrigger("%CourseDirectoryImportTrigger%"
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
            logger.LogInformation("Course directory scheduled import function was called.");

            var (savedQualifications, deletedQualifications) = await _courseDirectoryDataService.ImportQualificationsFromCourseDirectoryApi();
            logger.LogInformation("Course directory import inserted or updated {savedQualifications} and deleted {deletedQualifications} qualifications.", savedQualifications, deletedQualifications);

            var (savedProviders, deletedProviders) =
                await _courseDirectoryDataService.ImportProvidersFromCourseDirectoryApi();
            logger.LogInformation("Course directory import inserted or updated {savedProviders} and deleted {deletedProviders} providers.", savedProviders, deletedProviders);

            logger.LogInformation("Course directory scheduled import finished.");
        }
        catch (Exception e)
        {
            logger.LogError("Error importing data from course directory. Internal Error Message {e}", e);
        }
    }

    [Function("GetCourseDirectoryJson")]
    public async Task<HttpResponseData> GetCourseDirectoryDetailJson(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequestData request,
        FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger("HttpFunction");

        try
        {
            logger.LogInformation("Course directory GetCourseDirectoryDetailJson function was called.");

            var json = await _courseDirectoryDataService.GetTLevelDetailJsonFromCourseDirectoryApi();

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(json);

            return response;
        }
        catch (Exception e)
        {
            logger.LogError("Error reading json data from course directory. Internal Error Message {e}", e);

            return request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    [Function("GetCourseDirectoryQualificationJson")]
    public async Task<HttpResponseData> GetCourseDirectoryQualificationJson(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequestData request,
        FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger("HttpFunction");

        try
        {
            logger.LogInformation("Course directory GetCourseDirectoryQualificationJson function was called.");

            var json = await _courseDirectoryDataService.GetTLevelQualificationJsonFromCourseDirectoryApi();

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(json);

            return response;
        }
        catch (Exception e)
        {
            logger.LogError("Error reading json data from course directory. Internal Error Message {e}", e);

            return request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    [Function("GetProviders")]
    public async Task<HttpResponseData> GetProviders(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequestData request,
        FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger("HttpFunction");

        try
        {
            logger.LogInformation("Course directory GetProviders function was called.");

            var providers =
                (await _tableStorageService.GetAllProviders())
                .OrderBy(p => p.UkPrn)
                .ToList();

            logger.LogInformation("Course directory GetProviders returned {providers.Count} records.", providers.Count);

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            var json = JsonSerializer.Serialize(providers,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            await response.WriteStringAsync(json);

            return response;
        }
        catch (Exception e)
        {
            logger.LogError("Error in GetProviders. Internal Error Message {e}", e);

            return request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    [Function("GetQualifications")]
    public async Task<HttpResponseData> GetQualifications(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequestData request,
        FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger("HttpFunction");

        try
        {
            logger.LogInformation("Course directory GetQualifications function was called.");

            var qualifications =
                (await _tableStorageService.GetAllQualifications())
                .OrderBy(q => q.Id)
                .ToList();

            logger.LogInformation("Course directory GetQualifications returned {qualifications.Count} records.", qualifications.Count);

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            var json = JsonSerializer.Serialize(qualifications,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            await response.WriteStringAsync(json);

            return response;
        }
        catch (Exception e)
        {
            logger.LogError("Error in GetQualifications. Internal Error Message {e}", e);

            return request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}