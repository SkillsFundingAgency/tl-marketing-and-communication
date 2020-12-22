using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace sfa.Tl.Marketing.Communication.Functions
{
    public class CourseDirectoryImportFunction
    {
        [FunctionName("ManualImport")]
        public async Task<IActionResult> ManualImport(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("ManualImport::C# HTTP trigger function processed a request.");

            return new OkResult();
        }
    }
}
