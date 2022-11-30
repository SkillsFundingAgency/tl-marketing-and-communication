using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests.Builders;

public static class TownDataImportFunctionsBuilder
{
    public static TownDataImportFunctions Build(
        ITownDataService townDataService = null,
        ITableStorageService tableStorageService = null)
    {
        townDataService ??= Substitute.For<ITownDataService>();
        tableStorageService ??= Substitute.For<ITableStorageService>();

        return new TownDataImportFunctions(townDataService, tableStorageService);
    }
}