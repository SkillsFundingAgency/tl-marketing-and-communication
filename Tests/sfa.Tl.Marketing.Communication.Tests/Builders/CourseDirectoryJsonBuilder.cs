using sfa.Tl.Marketing.Communication.Application.Extensions;

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

        public string BuildValidTLevelQualificationsResponse() =>
            $"{GetType().Namespace}.Data.TLevelQualificationsResponse.json"
                .ReadManifestResourceStreamAsString();
    }
}
