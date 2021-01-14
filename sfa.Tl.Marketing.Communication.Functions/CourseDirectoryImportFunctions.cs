using System;
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

        public CourseDirectoryImportFunctions(ICourseDirectoryDataService courseDirectoryDataService)
        {
            _courseDirectoryDataService = courseDirectoryDataService ?? throw new ArgumentNullException(nameof(courseDirectoryDataService));
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

                var resultsCount = await _courseDirectoryDataService.ImportFromCourseDirectoryApi();

                logger.LogInformation($"Course directory scheduled import saved {resultsCount} records.");
            }
            catch (Exception e)
            {
                var errorMessage = $"Error importing data from course directory. Internal Error Message {e}";
                logger.LogError(errorMessage);
            }
        }

        [FunctionName("CourseDirectoryImportFunction")]
        public async Task<IActionResult> ManualImport(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest request,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("Course directory ManualImport function was called.");

                var resultsCount = await _courseDirectoryDataService.ImportFromCourseDirectoryApi();

                logger.LogInformation($"Course directory ManualImport saved {resultsCount} records.");

                return new OkObjectResult($"{resultsCount} records saved.");
            }
            catch (Exception e)
            {
                var errorMessage = $"Error importing data from course directory. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return new InternalServerErrorResult();
            }
        }

        public async Task<IActionResult> GetProviders(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest request,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("Course directory GetProviders function was called.");

                var providers = await _courseDirectoryDataService.GetProviders();

                logger.LogInformation($"Course directory GetProviders returned {providers?.Count} records.");

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

                var qualifications = await _courseDirectoryDataService.GetQualifications();

                logger.LogInformation($"Course directory GetQualifications returned {qualifications?.Count} records.");
                
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
