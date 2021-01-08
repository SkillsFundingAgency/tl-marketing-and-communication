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
    public class CourseDirectoryImportFunction
    {
        //[FunctionName("SendProviderFeedbackEmails")]
        public async Task ImportCourseDirectoryData(
            //[TimerTrigger("%CourseDirectoryImportTrigger%")]
            TimerInfo timer,
            ExecutionContext context,
            ICourseDirectoryDataService courseDirectoryDataService,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("Course directory scheduled import function was called.");

                var resultsCount = await courseDirectoryDataService.ImportFromCourseDirectoryApi();

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
            HttpRequest req,
            ICourseDirectoryDataService courseDirectoryDataService,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("Course directory ManualImport function was called.");

                var resultsCount = await courseDirectoryDataService.ImportFromCourseDirectoryApi();

                logger.LogInformation("Course directory ManualImport saved {resultsCount} records.");

                return new OkObjectResult($"{resultsCount} records saved.");
            }
            catch (Exception e)
            {
                var errorMessage = $"Error importing data from course directory. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return new InternalServerErrorResult();
            }
        }
    }
}
