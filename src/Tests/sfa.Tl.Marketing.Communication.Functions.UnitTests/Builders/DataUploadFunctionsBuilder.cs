using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests.Builders;

public static class DataUploadFunctionsBuilder
{
    public static DataUploadFunctions Build(
        IBlobStorageService blobStorageService = null)
    {
        blobStorageService ??= Substitute.For<IBlobStorageService>();

        return new DataUploadFunctions(blobStorageService);
    }
}