using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
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
        public void GetQualifications_By_Ids_Returns_Qualifications_By_Ids()
        {
            var ids = new[] { 37, 39, 40 };
            var results = _providerDataService.GetQualifications(ids).ToList();

            results.Count.Should().Be(3);
            results.SingleOrDefault(q => q.Id == 37).Should().NotBeNull();
            results.SingleOrDefault(q => q.Id == 39).Should().NotBeNull();
            results.SingleOrDefault(q => q.Id == 40).Should().NotBeNull();

            results.Single(q => q.Id == 37).Name.Should().Be("Digital Production, Design and Development");
            results.Single(q => q.Id == 39).Name.Should().Be("Digital Business Services");
            results.Single(q => q.Id == 40).Name.Should().Be("Digital Support Services");
        }

        [Fact]
        public void GetQualifications_ByIds_Returns_Qualifications_In_Alphabetical_Order()
        {
            var ids = new[] { 36, 37, 38, 39, 40, 41, 42, 43, 44, 45 };

            var results = _providerDataService
                .GetQualifications(ids)
                .OrderBy(q => q.Name)
                .ToList();

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
            const int id = 43;

            var result = _providerDataService.GetQualification(id);

            result.Id.Should().Be(id);
            result.Name.Should().Be("Science");
        }

        [Fact]
        public void GetWebsiteUrls_Returns_Expected_Number_Of_Urls()
        {
            var results = _providerDataService.GetWebsiteUrls();

            results.Count.Should().Be(12);
        }

        [Fact]
        public void GetWebsiteUrls_Returns_Urls_With_No_Duplicates()
        {
            var results = _providerDataService.GetWebsiteUrls();

            foreach (var item in results)
            {
                results.Keys.Count(x => x == item.Value).Should().Be(1);
            }
        }

        private static IProviderDataService CreateProviderDataService(
            ITableStorageService tableStorageService = null,
            IMemoryCache cache = null,
            ConfigurationOptions configuration = null)
        {
            tableStorageService ??= Substitute.For<ITableStorageService>();
            cache ??= Substitute.For<IMemoryCache>();
            configuration ??= new ConfigurationOptions
            {
                CacheExpiryInSeconds = 1
            };

            return new ProviderDataService(tableStorageService, cache, configuration);
        }
    }
}
