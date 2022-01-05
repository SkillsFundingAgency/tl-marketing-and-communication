using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Extensions
{
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
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void TempProviderDataExtensions_Loads_First_Known_Provider()
        {
            var knownProvider = TempProviderDataExtensions
                .ProviderData
                .SingleOrDefault(p => p.Key == 10035123)
                .Value;

            knownProvider.Should().NotBeNull();
            knownProvider.Name.Should().Be("BIDDULPH HIGH SCHOOL");
            knownProvider.Locations.Should().NotBeNullOrEmpty();

            var knownLocation = knownProvider.Locations.First();
            knownLocation.Postcode.Should().Be("ST8 7AR");
            knownLocation.Town.Should().Be("STOKE-ON-TRENT");
            knownLocation.Latitude.Should().Be(53.105857);
            knownLocation.Longitude.Should().Be(-2.171092);
            knownLocation.Website.Should().Be("https://biddulphhigh.co.uk/");
            knownLocation.DeliveryYears.Should().NotBeNullOrEmpty();

            var knownDeliveryYear = knownLocation.DeliveryYears.First();
            knownDeliveryYear.Year.Should().Be(2022);
            knownDeliveryYear.Qualifications.Should().NotBeNullOrEmpty();
            knownDeliveryYear.Qualifications.First().Should().Be(41);
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
            expectedCount -= 1; //TODO: Remove this, just here to balance temp test data

            var result = providers
                .MergeTempProviders(true);

            result.Should().NotBeNullOrEmpty();

            result.Count.Should().Be(expectedCount);
        }
    }
}
