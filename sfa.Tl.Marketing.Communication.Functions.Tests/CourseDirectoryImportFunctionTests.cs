using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace sfa.Tl.Marketing.Communication.Functions.Tests
{
    public class CourseDirectoryImportFunctionTests
    {
        [Fact]
        public async Task CourseDirectoryImportFunction_Does_Something()
        {
            var function = new CourseDirectoryImportFunction();

            var logger = Substitute.For<ILogger>();

            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Method = HttpMethod.Get.ToString();

            var result = await function.ManualImport(request, logger);

            result.Should().BeOfType<OkResult>();
        }
    }
}
