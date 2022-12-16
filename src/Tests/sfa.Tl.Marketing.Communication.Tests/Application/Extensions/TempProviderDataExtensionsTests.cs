using System.Collections.Generic;
using System.Linq;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Extensions;

public class TempProviderDataExtensionsTests
{
    [Fact]
    public void TempProviderDataExtensions_Loads_First_Known_Provider()
    {
        var tempProvidersJsonDocument = new TestProvidersFromJsonBuilder()
            .BuildJsonDocument();

        var results = new List<Provider>()
            .MergeTempProviders(tempProvidersJsonDocument, true);
        
        var knownProvider = results
            .SingleOrDefault(p =>
                p.UkPrn == 10000055);

        knownProvider.Should().NotBeNull();
        // ReSharper disable once StringLiteralTypo
        knownProvider!.Name.Should().Be("Abingdon and Witney College");
        knownProvider.Locations.Should().NotBeNullOrEmpty();

        var knownLocation = knownProvider.Locations.First();
        knownLocation.Postcode.Should().Be("OX14 1GG");
        knownLocation.Town.Should().Be("Vale of White Horse");
        knownLocation.Latitude.Should().Be(51.680624);
        knownLocation.Longitude.Should().Be(-1.28696);
        knownLocation.Website.Should().Be("https://www.abingdon-witney.ac.uk/whats-new/t-levels");

        knownLocation.DeliveryYears.Should().NotBeNullOrEmpty();

        var knownDeliveryYear = knownLocation.DeliveryYears.First();
        knownDeliveryYear.Year.Should().Be(2021);
        knownDeliveryYear.Qualifications.Should().NotBeNullOrEmpty();
        knownDeliveryYear.Qualifications.First().Should().Be(4);
    }

    [Fact]
    public void TempProviderDataExtensions_MergeTempProviders_Does_Not_Merge_When_Should_Merge_Is_False()
    {
        var providers = new List<Provider>();

        var tempProvidersJsonDocument = new TestProvidersFromJsonBuilder()
            .BuildJsonDocument();

        var result = providers
            .MergeTempProviders(tempProvidersJsonDocument);

        result.Should().BeEmpty();
    }

    [Fact]
    public void TempProviderDataExtensions_Merges_Data_Into_Empty_List()
    {
        var providersFromJsonBuilder = new TestProvidersFromJsonBuilder();
        var tempProviders = providersFromJsonBuilder
            .Build();
        var tempProvidersJsonDocument = providersFromJsonBuilder
            .BuildJsonDocument();

        var providers = new List<Provider>();

        var result = providers
            .MergeTempProviders(tempProvidersJsonDocument, true);

        result.Should().NotBeNullOrEmpty();

        var expectedCount = tempProviders.Count;

        result.Count.Should().Be(expectedCount);
    }

    [Fact]
    public void TempProviderDataExtensions_Merges_Data_Into_Existing_List()
    {
        var providers = new ProviderListBuilder()
            .CreateKnownList()
            .Build();

        var providersFromJsonBuilder = new TestProvidersFromJsonBuilder();
        var tempProviders = providersFromJsonBuilder
            .Build();
        var tempProvidersJsonDocument = providersFromJsonBuilder
            .BuildJsonDocument();

        var tempProvidersCount = tempProviders.Count;
        var originalProvidersCount = providers.Count;
        
        var duplicatesCount = tempProviders.Count(
            t => providers.Any(p=> p.UkPrn == t.UkPrn));

        var expectedCount = tempProvidersCount + originalProvidersCount - duplicatesCount;

        var result = providers
            .MergeTempProviders(tempProvidersJsonDocument, true);

        result.Should().NotBeNullOrEmpty();

        duplicatesCount.Should().Be(1);
        result.Count.Should().Be(expectedCount);
    }
}