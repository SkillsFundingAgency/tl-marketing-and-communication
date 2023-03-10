using System.IO;
using sfa.Tl.Marketing.Communication.Application.Extensions;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

public class IndexOfPlaceNamesCsvBuilder
{
    public Stream BuildIndexOfPlaceNamesCsvAsStream() =>
        $"{GetType().Namespace}.Data.IndexOfPlaceNames.csv"
            .ReadManifestResourceStream();
}