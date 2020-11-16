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

            var dataFilePath = Path.Combine(basePath ?? string.Empty, "Data", "data.json");
            var configurationOption = new ConfigurationOptions()
            {
                DataFilePath = dataFilePath
            };
            _service = new ProviderDataService(fileReader, configurationOption);
        }

        [Fact]
        public void GetProviders_Returns_All_Providers()
        {
            // Arrange
            // Act
            var providers = _service.GetProviders();
            // Assert
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
        }

        [Fact]
        public void GetQualifications_Returns_All_Qualifications_From_DataFile_And_Add_A_Default_To_Show_All()
        {
            // Arrange
            // Act
            var results = _service.GetQualifications().ToList();
            // Assert
            results.Count.Should().Be(11);
            results.SingleOrDefault(q => q.Id == 0 && q.Name == "All T Level courses").Should().NotBeNull();
        }

        [Fact]
        public void GetQualifications_Returns_Qualifications_By_Ids()
        {
            // Arrange
            var ids = new[] { 3, 4, 5 };

            // Act
            var results = _service.GetQualifications(ids).ToList();

            // Assert
            results.Count.Should().Be(3);
            results.Single(q => q.Id == 3).Should().NotBeNull();
            results.Single(q => q.Id == 4).Should().NotBeNull();
            results.Single(q => q.Id == 5).Should().NotBeNull();
        }

        [Fact]
        public void GetQualification_Returns_A_Qualification_By_Id()
        {
            // Arrange
            var id = 10;

            // Act
            var result = _service.GetQualification(id);

            // Assert
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
