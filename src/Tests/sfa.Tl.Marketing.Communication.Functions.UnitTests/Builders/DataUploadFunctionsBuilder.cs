using System.IO;
using System.Reflection;
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
    
    public static Stream BuildFormDataStream() =>
        Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(
                $"{typeof(DataUploadFunctionsBuilder).Namespace}.Data.TestMultipartFormData.txt");
}