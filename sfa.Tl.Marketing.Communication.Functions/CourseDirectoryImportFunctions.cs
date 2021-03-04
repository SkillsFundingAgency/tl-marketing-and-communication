using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
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

        [FunctionName("CourseDirectoryScheduledImport")]
        public async Task ImportCourseDirectoryData(
            [TimerTrigger("%CourseDirectoryImportTrigger%")]
            TimerInfo timer,
            ExecutionContext context,
            ILogger logger)
        {
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

        [FunctionName("CourseDirectoryManualImport")]
        public async Task<IActionResult> ManualImport(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest request,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("Course directory manual import function was called.");

                var (savedProviders, deletedProviders, savedQualifications, deletedQualifications) = await Import(logger);

                logger.LogInformation("Course directory manual import finished.");

                return new OkObjectResult(
                    $"Inserted or updated {savedProviders} and deleted {deletedProviders} providers.\r\n" +
                    $"Inserted or updated {savedQualifications} and deleted {deletedQualifications} qualifications.");
            }
            catch (Exception e)
            {
                var errorMessage = $"Error importing data from course directory. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return new InternalServerErrorResult();
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

        [FunctionName("GetCourseDirectoryJson")]
        public async Task<IActionResult> GetCourseDirectoryDetailJson(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest request,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("Course directory GetCourseDirectoryDetailJson function was called.");

                var json = await _courseDirectoryDataService.GetTLevelDetailJsonFromCourseDirectoryApi();

                return new ContentResult
                {
                    Content = json,
                    ContentType = "application/json"
                };
            }
            catch (Exception e)
            {
                var errorMessage = $"Error reading json data from course directory. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return new InternalServerErrorResult();
            }
        }

        [FunctionName("GetCourseDirectoryQualificationJson")]
        public async Task<IActionResult> GetCourseDirectoryQualificationJson(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest request,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("Course directory GetCourseDirectoryQualificationJson function was called.");

                var json = await _courseDirectoryDataService.GetTLevelQualificationJsonFromCourseDirectoryApi();

                return new ContentResult
                {
                    Content = json,
                    ContentType = "application/json"
                };
            }
            catch (Exception e)
            {
                var errorMessage = $"Error reading json data from course directory. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return new InternalServerErrorResult();
            }
        }

        [FunctionName("GetProviders")]
        public async Task<IActionResult> GetProviders(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest request,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("Course directory GetProviders function was called.");

                var providers = 
                    (await _tableStorageService.GetAllProviders())
                    .OrderBy(p => p.UkPrn);

                logger.LogInformation($"Course directory GetProviders returned {providers.Count()} records.");

                return new JsonResult(providers);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error in GetProviders. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return new InternalServerErrorResult();
            }
        }

        [FunctionName("GetQualifications")]
        public async Task<IActionResult> GetQualifications(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest request,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("Course directory GetQualifications function was called.");

                var qualifications = 
                    (await _tableStorageService.GetAllQualifications())
                    .OrderBy(q => q.Id);

                logger.LogInformation($"Course directory GetQualifications returned {qualifications.Count()} records.");

                return new JsonResult(qualifications);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error in GetQualifications. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return new InternalServerErrorResult();
            }
        }
    }
}
