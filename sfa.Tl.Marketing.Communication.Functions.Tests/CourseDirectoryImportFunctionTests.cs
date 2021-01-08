using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using Xunit;

namespace sfa.Tl.Marketing.Communication.Functions.Tests
{
    public class CourseDirectoryImportFunctionTests
    {
        [Fact]
        public async Task CourseDirectoryImportFunction_Scheduled_Import_Calls_CourseDirectoryImportService()
        {
            var function = new CourseDirectoryImportFunction();

            var service = Substitute.For<ICourseDirectoryDataService>();
            service.ImportFromCourseDirectoryApi().Returns(10);

            var timerSchedule = Substitute.For<TimerSchedule>();
            var logger = new NullLogger<CourseDirectoryImportFunction>();

            await function.ImportCourseDirectoryData(
                new TimerInfo(timerSchedule, new ScheduleStatus()),
                new ExecutionContext(),
                service,
                logger);

            await service.Received(1).ImportFromCourseDirectoryApi();
        }

        [Fact]
        public async Task CourseDirectoryImportFunction_ManualImport_Calls_CourseDirectoryImportService()
        {
            var function = new CourseDirectoryImportFunction();

            var service = Substitute.For<ICourseDirectoryDataService>();
            service.ImportFromCourseDirectoryApi().Returns(10);

            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Method = HttpMethod.Get.ToString();

            var logger = Substitute.For<ILogger>();

            var result = await function.ManualImport(request, service, logger);

            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be("10 records saved.");

            await service.Received(1).ImportFromCourseDirectoryApi();
        }
    }
}
