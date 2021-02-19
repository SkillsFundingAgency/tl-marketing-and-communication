using sfa.Tl.Marketing.Communication.Application.Extensions;
// ReSharper disable All

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    internal class CourseDirectoryJsonBuilder
    {
        public string BuildValidTLevelDetailSingleItemResponse() =>
            $"{GetType().Namespace}.Data.TLevelDetailSingleItemResponse.json"
                .ReadManifestResourceStreamAsString();

        public string BuildValidTLevelDetailMultiItemResponse() =>
            $"{GetType().Namespace}.Data.TLevelDetailMultiItemResponse.json"
                .ReadManifestResourceStreamAsString();

        public string BuildValidTLevelDefinitionsResponse() =>
            $"{GetType().Namespace}.Data.tleveldefinitions.json"
                .ReadManifestResourceStreamAsString();
    }
}
