using sfa.Tl.Marketing.Communication.Application.Extensions;
// ReSharper disable All

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

internal class FindProviderApiJsonBuilder
{
    public string BuildGetAllProvidersResponse() =>
        $"{GetType().Namespace}.Data.find_providers_get_all_providers.json"
            .ReadManifestResourceStreamAsString();

    public string BuildGetQualificationsResponse() =>
        $"{GetType().Namespace}.Data.find_providers_get_qualifications.json"
            .ReadManifestResourceStreamAsString();
    }