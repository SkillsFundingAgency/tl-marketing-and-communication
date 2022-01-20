using sfa.Tl.Marketing.Communication.Application.Extensions;
// ReSharper disable All

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    internal class PostcodeResponseJsonBuilder
    {
        public string BuildValidPostcodeResponse(string postcode) =>
            $"{GetType().Namespace}.Data.postcode_response_{postcode.ToLower().Replace(' ', '_')}.json"
                .ReadManifestResourceStreamAsString();

        public string BuildTerminatedPostcodeResponse(string postcode) =>
            $"{GetType().Namespace}.Data.terminated_postcode_response_{postcode.ToLower().Replace(' ', '_')}.json"
                .ReadManifestResourceStreamAsString();

        public string BuildOutcodeResponse(string postcode) =>
            $"{GetType().Namespace}.Data.outcode_response_{postcode.ToLower().Replace(' ', '_')}.json"
                .ReadManifestResourceStreamAsString();

        public string BuildPostcodeNotFoundResponse() =>
            $"{GetType().Namespace}.Data.postcode_not_found_response.json"
                .ReadManifestResourceStreamAsString();
    }
}
