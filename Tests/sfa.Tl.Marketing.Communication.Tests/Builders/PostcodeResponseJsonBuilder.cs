using sfa.Tl.Marketing.Communication.Application.Extensions;
// ReSharper disable All

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    internal class PostcodeResponseJsonBuilder
    {
        public string BuildValidPostcodeResponse(string postcode) =>
            $"{GetType().Namespace}.Data.postcode_response_{postcode.ToLower().Replace(' ', '_')}.json"
                .ReadManifestResourceStreamAsString();

        public string BuildVTerminatedPostcodeResponse(string postcode) =>
            $"{GetType().Namespace}.Data.terminated_postcode_response_{postcode.ToLower().Replace(' ', '_')}.json"
                .ReadManifestResourceStreamAsString();
    }
}
