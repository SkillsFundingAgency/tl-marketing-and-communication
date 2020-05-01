using FluentAssertions;
using NSubstitute;
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
        private readonly IJsonConvertor _jsonConvertor;
        private readonly IFileReader _fileReader;
        private readonly ConfigurationOptions _configurationOption;

        public ProviderDataServiceUnitTests()
        {
            _jsonConvertor = new JsonConvertor();
            _fileReader = new FileReader();
            var appDomain = System.AppDomain.CurrentDomain;
            var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;
            var dataFilePath = Path.Combine(basePath, "Data", "data.json");
            _configurationOption = new ConfigurationOptions()
            {
                DataFilePath = dataFilePath
            };
            _service = new ProviderDataService(_fileReader, _jsonConvertor, _configurationOption);
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
        public void GetQualifications_Returns_All_Qualifications_From_DataFile_And_Add_A_Default_To_Show_All()
        {
            // Arrange
            // Act
            var results = _service.GetQualifications();
            // Assert
            results.Count().Should().Be(11);
            results.SingleOrDefault(q => q.Id == 0 && q.Name == "All T Level courses").Should().NotBeNull();
        }

        [Fact]
        public void GetQualifications_Returns_Qualifications_By_Ids()
        {
            // Arrange
            int[] ids = new int[] { 3, 4, 5 };

            // Act
            var results = _service.GetQualifications(ids);

            // Assert
            results.Count().Should().Be(3);
            results.Single(q => q.Id == 3).Should().NotBeNull();
            results.Single(q => q.Id == 4).Should().NotBeNull();
            results.Single(q => q.Id == 5).Should().NotBeNull();
        }

        [Fact]
        public void GetQualification_Returns_A_Qualification_By_Id()
        {
            // Arrange
            int id = 10;

            // Act
            var result = _service.GetQualification(id);

            // Assert
            result.Id.Should().Be(id);
            result.Name.Should().Be("Science");
        }
    }
}
