using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services
{
    public class ProviderDataServiceUnitTests
    {
        private readonly IProviderDataService _providerDataService;

        public ProviderDataServiceUnitTests()
        {
            var qualifications = new TestQualificationsFromJsonBuilder()
                .Build();
            var providers = new TestProvidersFromJsonBuilder()
                .Build();
            var tableStorageService = Substitute.For<ITableStorageService>();
            tableStorageService.GetAllProviders().Returns(providers);
            tableStorageService.GetAllQualifications().Returns(qualifications);

            _providerDataService = CreateProviderDataService(tableStorageService);
        }

        [Fact]
        public void GetProviders_Returns_All_Providers()
        {
            var providers = _providerDataService.GetProviders();

            providers.Count().Should().Be(10);
        }

        [Fact]
        public void GetProviders_Returns_Expected_First_Provider()
        {
            var providers = _providerDataService.GetProviders();

            var provider = providers.SingleOrDefault(p => p.UkPrn == 10000055);

            provider.Should().NotBeNull();

            provider?.UkPrn.Should().Be(10000055);
            provider?.Name.Should().Be("Abingdon and Witney College");
            provider?.Locations.Count.Should().Be(2);

            var location = provider?.Locations.First();
            location?.Name.Should().Be("");
            location?.Postcode.Should().Be("OX14 1GG");
            location?.Town.Should().Be("Vale of White Horse");
            location?.Latitude.Should().Be(51.680624);
            location?.Longitude.Should().Be(-1.28696);
            location?.Website.Should().Be("https://www.abingdon-witney.ac.uk/whats-new/t-levels");

            location?.DeliveryYears.Count.Should().Be(1);
            var deliveryYear = location?.DeliveryYears.First();
            deliveryYear.Should().NotBeNull();
            deliveryYear?.Year.Should().Be(2021);

            deliveryYear?.Qualifications.Should().NotBeNull();
            deliveryYear?.Qualifications.Should().Contain(4);
            deliveryYear?.Qualifications.Should().Contain(6);
            deliveryYear?.Qualifications.Should().Contain(7);
        }

        [Fact]
        public void GetProviders_Returns_Expected_Provider_Location_Delivery_Years()
        {
            var providers = _providerDataService.GetProviders();

            var provider = providers.SingleOrDefault(p => p.UkPrn == 10000754);

            provider.Should().NotBeNull();
            provider?.Name.Should().Be("Blackpool and The Fylde College");
            provider?.Locations.Count.Should().Be(1);

            var location = provider?.Locations.First();
            location?.Postcode.Should().Be("FY2 0HB");

            location?.DeliveryYears.Count.Should().Be(2);
            var deliveryYear2020 = location?.DeliveryYears.Single(d => d.Year == 2020);
            deliveryYear2020.Should().NotBeNull();
            deliveryYear2020?.Year.Should().Be(2020);

            deliveryYear2020?.Qualifications.Should().NotBeNull();
            deliveryYear2020?.Qualifications.Count.Should().Be(3);
            deliveryYear2020?.Qualifications.Should().Contain(2);
            deliveryYear2020?.Qualifications.Should().Contain(4);
            deliveryYear2020?.Qualifications.Should().Contain(6);

            var deliveryYear2021 = location?.DeliveryYears.Single(d => d.Year == 2021);
            deliveryYear2021.Should().NotBeNull();
            deliveryYear2021?.Year.Should().Be(2021);

            deliveryYear2021?.Qualifications.Count.Should().Be(6);
            deliveryYear2021?.Qualifications.Should().NotBeNull();
            deliveryYear2021?.Qualifications.Should().Contain(5);
            deliveryYear2021?.Qualifications.Should().Contain(3);
            deliveryYear2021?.Qualifications.Should().Contain(9);
            deliveryYear2021?.Qualifications.Should().Contain(1);
            deliveryYear2021?.Qualifications.Should().Contain(7);
            deliveryYear2021?.Qualifications.Should().Contain(10);
        }

        [Fact]
        public void GetQualifications_Returns_All_Qualifications_From_Data_And_Adds_A_Default_To_Show_All()
        {
            var results = _providerDataService.GetQualifications().ToList();
            results.Count.Should().Be(11);
            results.SingleOrDefault(q => q.Id == 0 && q.Name == "All T Level courses").Should().NotBeNull();
        }

        [Fact]
        public void GetQualifications_By_Ids_Returns_Qualifications_By_Ids()
        {
            var ids = new[] { 3, 4, 5 };
            var results = _providerDataService.GetQualifications(ids).ToList();

            results.Count.Should().Be(3);
            results.Single(q => q.Id == 3).Should().NotBeNull();
            results.Single(q => q.Id == 4).Should().NotBeNull();
            results.Single(q => q.Id == 5).Should().NotBeNull();
        }

        [Fact]
        public void GetQualifications_ByIds_Returns_Qualifications_In_Alphabetical_Order()
        {
            var ids = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var results = _providerDataService.GetQualifications(ids).ToList();

            results.Count.Should().Be(10);
            results[0].Name.Should().Be("Building Services Engineering");
            results[1].Name.Should().Be("Design, Surveying and Planning for Construction");
            results[2].Name.Should().Be("Digital Business Services");
            results[3].Name.Should().Be("Digital Production, Design and Development");
            results[4].Name.Should().Be("Digital Support Services");
            results[5].Name.Should().Be("Education and Childcare");
            results[6].Name.Should().Be("Health");
            results[7].Name.Should().Be("Healthcare Science");
            results[8].Name.Should().Be("Onsite Construction");
            results[9].Name.Should().Be("Science");
        }

        [Fact]
        public void GetQualification_Returns_A_Qualification_By_Id()
        {
            const int id = 10;

            var result = _providerDataService.GetQualification(id);

            result.Id.Should().Be(id);
            result.Name.Should().Be("Science");
        }

        [Fact]
        public void GetWebsiteUrls_Returns_Expected_Number_Of_Urls()
        {
            var results = _providerDataService.GetWebsiteUrls();

            results.Count().Should().Be(12);
        }

        [Fact]
        public void GetWebsiteUrls_Returns_Urls_With_No_Duplicates()
        {
            var results = _providerDataService.GetWebsiteUrls().ToList();

            foreach (var url in results)
            {
                results.Count(x => x == url).Should().Be(1);
            }
        }

        private static IProviderDataService CreateProviderDataService(
            ITableStorageService tableStorageService = null,
            IMemoryCache cache = null,
            ILogger<ProviderDataService> logger = null,
            ConfigurationOptions configuration = null)
        {
            tableStorageService ??= Substitute.For<ITableStorageService>();
            cache ??= Substitute.For<IMemoryCache>();
            logger ??= Substitute.For<ILogger<ProviderDataService>>();
            configuration ??= new ConfigurationOptions
            {
                CacheExpiryInSeconds = 1
            };

            return new ProviderDataService(tableStorageService, cache, logger, configuration);
        }
    }
}
