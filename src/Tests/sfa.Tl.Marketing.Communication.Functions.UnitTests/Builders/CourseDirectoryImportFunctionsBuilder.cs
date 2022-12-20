using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests.Builders;

public static class CourseDirectoryImportFunctionsBuilder
{
    public static CourseDirectoryImportFunctions Build(
        ICourseDirectoryDataService courseDirectoryDataService = null,
        ITableStorageService tableStorageService = null)
    {
        courseDirectoryDataService ??= Substitute.For<ICourseDirectoryDataService>();
        tableStorageService ??= Substitute.For<ITableStorageService>();

        return new CourseDirectoryImportFunctions(courseDirectoryDataService, tableStorageService);
    }
}