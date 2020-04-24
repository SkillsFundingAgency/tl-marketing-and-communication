using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace sfa.Tl.Marketing.Communication.Application.UnitTests
{
    public class ProviderSearchServiceUnitTests
    {
        private readonly IProviderDataService _providerDataService;
        private readonly IProviderLocationService _providerLocationService;
        private readonly ILocationService _locationService;
        private readonly IDistanceCalculationService _distanceCalculationService;
        private readonly IProviderSearchService _service;

        public ProviderSearchServiceUnitTests()
        {
            _providerDataService = Substitute.For<IProviderDataService>();
            _providerLocationService = Substitute.For <IProviderLocationService>();
            _locationService = Substitute.For <ILocationService>();
            _distanceCalculationService = Substitute.For<IDistanceCalculationService>();

            _service = new ProviderSearchService(_providerDataService,
                _locationService,
                _providerLocationService,
                _distanceCalculationService);
        }

        [Fact]
        public void GetQualifications_Returns_All_Qualifications_OrderBy_Name()
        {
            // Arrange
            var qualifications = new List<Qualification>()
            {
                new Qualification { Id = 1, Name = "Xyz" },
                new Qualification { Id = 2, Name = "Mno" },
                new Qualification { Id = 3, Name = "Abc" },

            };

            _providerDataService.GetQualifications().Returns(qualifications);

            var expected = qualifications.OrderBy(x => x.Name); 
            // Act
            var actual = _service.GetQualifications();

            // Assert
            Assert.True(actual.SequenceEqual(expected));
            Assert.False(actual.SequenceEqual(qualifications));
            _providerDataService.Received(1).GetQualifications();
        }

        [Fact]
        public void GetQualificationById_Return_A_Qualifications_ById()
        {
            // Arrange
            const int id = 1;
            const string name = "ssss";

            var expected = new Qualification { Id = id, Name = name };

            _providerDataService.GetQualification(id).Returns(expected);
            // Act
            var actual = _service.GetQualificationById(id);

            // Assert
            actual.Should().BeSameAs(expected);
            _providerDataService.Received(1).GetQualification(id);
        }
        
        [Theory]
        [InlineData("mk128jk", true)]
        [InlineData("mk980kl", false)]
        public async Task IsSearchPostcodeValid_Validate_A_Postcode(string postcode, bool expected)
        {
            // Arrange
            _distanceCalculationService.IsPostcodeValid(postcode).Returns(expected);

            // Act
            var actual = await _service.IsSearchPostcodeValid(postcode);

            // Assert
            actual.Should().Be(expected);
            await _distanceCalculationService.Received(1).IsPostcodeValid(postcode);
        }

    }
}
