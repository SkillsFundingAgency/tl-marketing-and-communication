using sfa.Tl.Marketing.Communication.UnitTests.TestHelpers.Extensions;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    internal class CourseDirectoryJsonBuilder
    {
        public string BuildValidTLevelDetailResponse()
        {
            return $"{GetType().Namespace}.Data.TLevelDetailResponse.json"
                .ReadManifestResourceStreamAsString();
        }
    }
}
