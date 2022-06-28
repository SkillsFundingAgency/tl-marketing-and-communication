using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests.Builders;

public class CourseDirectoryImportFunctionsBuilder
{
    public CourseDirectoryImportFunctions BuildCourseDirectoryImportFunctions(
        ICourseDirectoryDataService courseDirectoryDataService = null,
        ITableStorageService tableStorageService = null)
    {
        courseDirectoryDataService ??= Substitute.For<ICourseDirectoryDataService>();
        tableStorageService ??= Substitute.For<ITableStorageService>();

        return new CourseDirectoryImportFunctions(courseDirectoryDataService, tableStorageService);
    }
}