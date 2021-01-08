using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.Services;
using System.Threading.Tasks;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services
{
    public class CourseDirectoryDataServiceTests
    {
        [Fact]
        public async Task CourseDirectoryDataService_Import_Returns_Expected_Result()
        {
            var service = new CourseDirectoryDataService();

            var result = await service.ImportFromCourseDirectoryApi();

            result.Should().Be(0);
        }
    }
}