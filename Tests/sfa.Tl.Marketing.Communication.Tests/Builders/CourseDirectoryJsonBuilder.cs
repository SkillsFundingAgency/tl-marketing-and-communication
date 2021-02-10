using sfa.Tl.Marketing.Communication.Application.Extensions;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    internal class CourseDirectoryJsonBuilder
    {
        public string BuildValidTLevelDetailResponse() =>
            $"{GetType().Namespace}.Data.TLevelDetailResponse.json"
                .ReadManifestResourceStreamAsString();

        public string BuildValidTLevelQualificationsResponse() =>
            $"{GetType().Namespace}.Data.TLevelQualificationsResponse.json"
                .ReadManifestResourceStreamAsString();
    }
}
