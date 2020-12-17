using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using System.IO;
using System.Linq;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services
{
    public class ProviderDataServiceUnitTests
    {
        private readonly IProviderDataService _service;

        public ProviderDataServiceUnitTests()
        {
            IFileReader fileReader = new FileReader();
            var appDomain = System.AppDomain.CurrentDomain;
            var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;

            var providersDataFilePath = Path.Combine(basePath!, "Data", "test_providers.json");
            var qualificationsDataFilePath = Path.Combine(basePath!, "Data", "test_qualifications.json");
            var configurationOption = new ConfigurationOptions()
            {
                ProvidersDataFilePath = providersDataFilePath,
                QualificationsDataFilePath = qualificationsDataFilePath
            };
            _service = new ProviderDataService(fileReader, configurationOption);
        }

        [Fact]
        public void GetProviders_Returns_All_Providers()
        {
            var providers = _service.GetProviders();

            providers.Count().Should().Be(10);
        }

        [Fact]
        public void GetProviders_Returns_Expected_First_Provider()
        {
            var providers = _service.GetProviders();

            var provider = providers.SingleOrDefault(p => p.Id == 1);

            provider.Should().NotBeNull();

            provider?.Id.Should().Be(1);
            provider?.Name.Should().Be("Abingdon and Witney College");
            provider?.Locations.Count.Should().Be(2);

            var location = provider?.Locations.First();
            location?.Name.Should().Be("");
            location?.Postcode.Should().Be("OX14 1GG");
            location?.Town.Should().Be("Vale of White Horse");
            location?.Latitude.Should().Be(51.680624);
            location?.Longitude.Should().Be(-1.28696);
            location?.Website.Should().Be("https://www.abingdon-witney.ac.uk/whats-new/t-levels");

            location?.Qualification2020.Length.Should().Be(0);
            location?.Qualification2021.Length.Should().Be(3);

            location?.Qualification2021.Should().Contain(4);
            location?.Qualification2021.Should().Contain(6);
            location?.Qualification2021.Should().Contain(7);

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
            var providers = _service.GetProviders();

            var provider = providers.SingleOrDefault(p => p.Id == 10);

            provider.Should().NotBeNull();
            provider?.Name.Should().Be("Blackpool and The Fylde College");
            provider?.Locations.Count.Should().Be(1);

            var location = provider?.Locations.First();
            location?.Postcode.Should().Be("FY2 0HB");

            location?.DeliveryYears.Count.Should().Be(2);
            var deliveryYear2020 = location?.DeliveryYears.First();
            deliveryYear2020.Should().NotBeNull();
            deliveryYear2020?.Year.Should().Be(2020);

            deliveryYear2020?.Qualifications.Should().NotBeNull();
            deliveryYear2020?.Qualifications.Count.Should().Be(3);
            deliveryYear2020?.Qualifications.Should().Contain(2);
            deliveryYear2020?.Qualifications.Should().Contain(4);
            deliveryYear2020?.Qualifications.Should().Contain(6);

            var deliveryYear2021 = location?.DeliveryYears.Skip(1).First();
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
        public void GetQualifications_Returns_All_Qualifications_From_DataFile_And_Add_A_Default_To_Show_All()
        {
            var results = _service.GetQualifications().ToList();
            results.Count.Should().Be(11);
            results.SingleOrDefault(q => q.Id == 0 && q.Name == "All T Level courses").Should().NotBeNull();
        }

        [Fact]
        public void GetQualifications_Returns_Qualifications_By_Ids()
        {
            var ids = new[] { 3, 4, 5 };

            var results = _service.GetQualifications(ids).ToList();

            results.Count.Should().Be(3);
            results.Single(q => q.Id == 3).Should().NotBeNull();
            results.Single(q => q.Id == 4).Should().NotBeNull();
            results.Single(q => q.Id == 5).Should().NotBeNull();
        }

        [Fact]
        public void GetQualification_Returns_A_Qualification_By_Id()
        {
            const int id = 10;

            var result = _service.GetQualification(id);

            result.Id.Should().Be(id);
            result.Name.Should().Be("Science");
        }

        [Fact]
        public void GetWebsiteUrls_Returns_Expected_Number_Of_Urls()
        {
            var results = _service.GetWebsiteUrls();

            results.Count().Should().Be(12);
        }

        [Fact]
        public void GetWebsiteUrls_Returns_Urls_With_No_Duplicates()
        {
            var results = _service.GetWebsiteUrls().ToList();

            foreach (var url in results)
            {
                results.Count(x => x == url).Should().Be(1);
            }
        }
    }
}
