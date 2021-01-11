using sfa.Tl.Marketing.Communication.Data.UnitTests.TestHelpers.Extensions;

namespace sfa.Tl.Marketing.Communication.Data.UnitTests.Builders
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
