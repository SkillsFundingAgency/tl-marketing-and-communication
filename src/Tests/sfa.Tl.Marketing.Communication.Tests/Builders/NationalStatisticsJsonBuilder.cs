using sfa.Tl.Marketing.Communication.Application.Extensions;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

// ReSharper disable StringLiteralTypo
public class NationalStatisticsJsonBuilder
{
    public string BuildNationalStatisticsLocationsResponse() =>
        $"{GetType().Namespace}.Data.nationalstatisticslocationsresponse.json"
            .ReadManifestResourceStreamAsString();
}