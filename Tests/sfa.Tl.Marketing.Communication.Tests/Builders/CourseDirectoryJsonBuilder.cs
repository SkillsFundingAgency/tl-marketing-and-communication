using sfa.Tl.Marketing.Communication.Application.Extensions;
// ReSharper disable All

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    internal class CourseDirectoryJsonBuilder
    {
        public string BuildValidTLevelsSingleItemResponse() =>
            $"{GetType().Namespace}.Data.tlevels_single_item.json"
                .ReadManifestResourceStreamAsString();

        public string BuildValidTLevelsMultiItemResponse() =>
            $"{GetType().Namespace}.Data.tlevels_multiple_items.json"
                .ReadManifestResourceStreamAsString();

        public string BuildValidTLevelDefinitionsResponse() =>
            $"{GetType().Namespace}.Data.tleveldefinitions.json"
                .ReadManifestResourceStreamAsString();
    }
}
