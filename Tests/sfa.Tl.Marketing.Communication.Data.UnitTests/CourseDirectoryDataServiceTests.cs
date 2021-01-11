using System.Threading.Tasks;
using FluentAssertions;
using sfa.Tl.Marketing.Communication.Data.Services;
using Xunit;

namespace sfa.Tl.Marketing.Communication.Data.UnitTests
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