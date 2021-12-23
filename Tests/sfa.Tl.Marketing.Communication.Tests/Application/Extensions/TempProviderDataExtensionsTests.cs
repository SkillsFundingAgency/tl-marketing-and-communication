using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.Services;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Extensions
{
    public class TempProviderDataExtensionsTests
    {
        [Fact]
        public void TempProviderDataExtensions_Loads_Data()
        {
            TempProviderDataExtensions
                .ProviderData
                .Should()
                .NotBeNullOrEmpty();
        }
    }
}
