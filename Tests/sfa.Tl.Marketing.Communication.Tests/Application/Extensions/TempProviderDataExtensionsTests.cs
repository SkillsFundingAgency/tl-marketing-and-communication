using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Extensions;

public class TempProviderDataExtensionsTests
{
    [Fact]
    public void TempProviderDataExtensions_Loads_Data_Successfully()
    {
        TempProviderDataExtensions
            .ProviderData
            .Should()
            .NotBeNullOrEmpty();
    }

    [Fact]
    public void TempProviderDataExtensions_Loads_First_Known_Provider()
    {
        var knownProvider = TempProviderDataExtensions
            .ProviderData
            .SingleOrDefault(p => 
                p.Key == 10042223)
            .Value;

        knownProvider.Should().NotBeNull();
        // ReSharper disable once StringLiteralTypo
        knownProvider.Name.Should().Be("BURNTWOOD SCHOOL");
        knownProvider.Locations.Should().NotBeNullOrEmpty();

        var knownLocation = knownProvider.Locations.First();
        knownLocation.Postcode.Should().Be("SW17 0AQ");
        knownLocation.Town.Should().Be("London");
        knownLocation.Latitude.Should().Be(51.438125);
        knownLocation.Longitude.Should().Be(-0.180083);
        knownLocation.Website.Should().Be("https://www.burntwoodschool.com/");

        knownLocation.DeliveryYears.Should().NotBeNullOrEmpty();

        var knownDeliveryYear = knownLocation.DeliveryYears.First();
        knownDeliveryYear.Year.Should().Be(2022);
        knownDeliveryYear.Qualifications.Should().NotBeNullOrEmpty();
        knownDeliveryYear.Qualifications.First().Should().Be(38);
    }

    [Fact]
    public void TempProviderDataExtensions_MergeTempProviders_Does_Not_Merge_When_Should_Merge_Is_False()
    {
        var providers = new List<Provider>();

        var result = providers
            .MergeTempProviders();

        result.Should().BeEmpty();
    }

    [Fact]
    public void TempProviderDataExtensions_Merges_Data_Into_Empty_List()
    {
        TempProviderDataExtensions
            .ProviderData
            .Should()
            .NotBeNullOrEmpty();

        var providers = new List<Provider>();

        var result = providers
            .MergeTempProviders(true);

        result.Should().NotBeNullOrEmpty();

        var expectedCount = TempProviderDataExtensions
            .ProviderData
            .Count;

        result.Count.Should().Be(expectedCount);
    }

    [Fact]
    public void TempProviderDataExtensions_Merges_Data_Into_Existing_List()
    {
        TempProviderDataExtensions
            .ProviderData
            .Should()
            .NotBeNullOrEmpty();

        var providers = new ProviderListBuilder()
            .CreateKnownList()
            .Build();

        var tempProvidersCount = TempProviderDataExtensions
            .ProviderData
            .Count;
        var originalProvidersCount = providers.Count;
        var expectedCount = tempProvidersCount + originalProvidersCount;

        var result = providers
            .MergeTempProviders(true);

        result.Should().NotBeNullOrEmpty();

        result.Count.Should().Be(expectedCount);
    }
}