using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Functions
{
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

                await Import(logger);

                logger.LogInformation("Course directory scheduled import finished.");
            }
            catch (Exception e)
            {
                var errorMessage = $"Error importing data from course directory. Internal Error Message {e}";
                logger.LogError(errorMessage);
            }
        }

        [Function("CourseDirectoryManualImport")]
        public async Task<HttpResponseData> ManualImport(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequestData request,
            FunctionContext functionContext)
        {
            var logger = functionContext.GetLogger("HttpFunction");

            try
            {
                logger.LogInformation("Course directory manual import function was called.");

                var (savedProviders, deletedProviders, savedQualifications, deletedQualifications) = await Import(logger);

                logger.LogInformation("Course directory manual import finished.");

                var response = request.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/text");

                await response.WriteStringAsync(
                    $"Inserted or updated {savedProviders} and deleted {deletedProviders} providers.\r\n" +
                    $"Inserted or updated {savedQualifications} and deleted {deletedQualifications} qualifications.");

                return response;
            }
            catch (Exception e)
            {
                var errorMessage = $"Error importing data from course directory. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        private async Task<(int SavedProviders, int DeletedProviders, int SavedQualifications, int DeletedQualifications)> Import(ILogger logger)
        {
            var (savedQualifications, deletedQualifications) = await _courseDirectoryDataService.ImportQualificationsFromCourseDirectoryApi();
            logger.LogInformation($"Course directory import saved {savedQualifications} and deleted {deletedQualifications} qualifications.");

            var (savedProviders, deletedProviders) =
                await _courseDirectoryDataService.ImportProvidersFromCourseDirectoryApi();
            logger.LogInformation($"Course directory import saved {savedProviders} and deleted {deletedProviders} providers.");

            return (savedProviders, deletedProviders, savedQualifications, deletedQualifications);
        }

        [Function("GetCourseDirectoryJson")]
        public async Task<HttpResponseData> GetCourseDirectoryDetailJson(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
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
                var errorMessage = $"Error reading json data from course directory. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("GetCourseDirectoryQualificationJson")]
        public async Task<HttpResponseData> GetCourseDirectoryQualificationJson(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
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
                var errorMessage = $"Error reading json data from course directory. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("GetProviders")]
        public async Task<HttpResponseData> GetProviders(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
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

                logger.LogInformation($"Course directory GetProviders returned {providers.Count} records.");

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
                var errorMessage = $"Error in GetProviders. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("GetQualifications")]
        public async Task<HttpResponseData> GetQualifications(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequestData request,
            FunctionContext functionContext)
        {
            var logger = functionContext.GetLogger("HttpFunction");

            try
            {
                logger.LogInformation("Course directory GetQualifications function was called.");

                logger.LogInformation($"Running framework {RuntimeInformation.FrameworkDescription}.");
                logger.LogInformation($"FUNCTIONS_EXTENSION_VERSION = {Environment.GetEnvironmentVariable("FUNCTIONS_EXTENSION_VERSION")}.");
                logger.LogInformation($"FUNCTIONS_WORKER_RUNTIME = {Environment.GetEnvironmentVariable("FUNCTIONS_WORKER_RUNTIME")}.");
                logger.LogInformation($"WEBSITE_RUN_FROM_PACKAGE = {Environment.GetEnvironmentVariable("WEBSITE_RUN_FROM_PACKAGE")}.");

                var qualifications =
                    (await _tableStorageService.GetAllQualifications())
                    .OrderBy(q => q.Id)
                    .ToList();

                logger.LogInformation($"Course directory GetQualifications returned {qualifications.Count} records.");

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
                var errorMessage = $"Error in GetQualifications. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
