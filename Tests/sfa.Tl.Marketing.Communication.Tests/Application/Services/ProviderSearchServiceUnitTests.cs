﻿using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services
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
            _providerLocationService = Substitute.For<IProviderLocationService>();
            _locationService = Substitute.For<ILocationService>();
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
                new Qualification { Id = 3, Name = "Abc" }
            };

            _providerDataService.GetQualifications().Returns(qualifications);

            var expected = qualifications.OrderBy(x => x.Name);
            // Act
            var actual = _service.GetQualifications().ToList();

            // Assert
            Assert.True(actual.SequenceEqual(expected));
            Assert.False(actual.SequenceEqual(qualifications));
            _providerDataService.Received(1).GetQualifications();
        }

        [Fact]
        public void GetQualificationById_Returns_A_Qualification()
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
            _distanceCalculationService.IsPostcodeValid(postcode).Returns((expected, new PostcodeLocation { Postcode = postcode }));

            // Act
            var (isValid, postcodeLocation) = await _service.IsSearchPostcodeValid(postcode);

            // Assert
            isValid.Should().Be(expected);
            postcodeLocation.Should().NotBeNull();
            postcodeLocation.Postcode.Should().Be(postcode);
            await _distanceCalculationService.Received(1).IsPostcodeValid(postcode);
        }

        [Fact]
        public async Task Search_Returns_Empty_Providers_And_TotalRecordCount()
        {
            // Arrange
            _providerDataService.GetProviders().Returns(new List<Provider>().AsQueryable());

            // Act
            var (totalCount, searchResults) = await _service.Search(new SearchRequest());

            // Assert
            totalCount.Should().Be(0);
            searchResults.Count().Should().Be(0);
            _providerDataService.Received(1).GetProviders();
        }

        [Fact]
        public async Task Search_Returns_ProviderLocations_Filtered_By_Qualification_And_NumberOfItems()
        {
            // Arrange
            var providers = new List<Provider>
            {
                new Provider(),
                new Provider(),
                new Provider()
            }.AsQueryable();
            _providerDataService.GetProviders().Returns(providers);

            int? qualificationId = 2232;
            const int numberOfItems = 2;
            const string postcode = "mk669oo";
            var searchRequest = new SearchRequest
            {
                QualificationId = qualificationId,
                NumberOfItems = numberOfItems,
                Postcode = postcode,
                OriginLatitude = "1.5",
                OriginLongitude = "50"
            };

            var locations = new List<Location>
            {
                new Location(),
                new Location(),
                new Location()
            }.AsQueryable();
            _locationService.GetLocations(Arg.Is<IQueryable<Provider>>(p => p == providers), Arg.Is<int>(q => q == searchRequest.QualificationId.Value)).Returns(locations);

            var providerLocations = new List<ProviderLocation>
            {
                new ProviderLocation(),
                new ProviderLocation(),
                new ProviderLocation()
            }.AsQueryable();

            _providerLocationService.GetProviderLocations(Arg.Is<IQueryable<Location>>(l => l == locations), Arg.Is<IQueryable<Provider>>(p => p == providers)).Returns(providerLocations);

            _distanceCalculationService.CalculateProviderLocationDistanceInMiles(
                Arg.Is<PostcodeLocation>(p => p.Postcode == searchRequest.Postcode
                                              && p.Latitude.ToString() == searchRequest.OriginLatitude
                                              && p.Longitude.ToString() == searchRequest.OriginLongitude),
                providerLocations)
                .Returns(providerLocations.ToList());

            // Act
            var (totalCount, searchResults) = await _service.Search(searchRequest);

            // Assert
            totalCount.Should().Be(providerLocations.Count());
            searchResults.Count().Should().Be(numberOfItems);
            _providerDataService.Received(1).GetProviders();
            _locationService.Received(1).GetLocations(Arg.Is<IQueryable<Provider>>(p => p == providers), Arg.Is<int>(q => q == searchRequest.QualificationId.Value));
            _providerLocationService.Received(1).GetProviderLocations(Arg.Is<IQueryable<Location>>(l => l == locations), Arg.Is<IQueryable<Provider>>(p => p == providers));
            await _distanceCalculationService.Received(1).CalculateProviderLocationDistanceInMiles(
                Arg.Is<PostcodeLocation>(p => p.Postcode == searchRequest.Postcode),
                providerLocations);
        }

        [Theory]
        [InlineData(null, 1)]
        [InlineData(0, 2)]
        public async Task Search_Returns_All_ProviderLocations_When_Qualification_Filter_Is_Null_Or_Zero(int? qualificationId, int numberOfItems)
        {
            // Arrange
            var providers = new List<Provider>()
            {
                new Provider(),
                new Provider(),
                new Provider()
            }.AsQueryable();
            _providerDataService.GetProviders().Returns(providers);
            const string postcode = "mk669oo";

            var searchRequest = new SearchRequest
            {
                QualificationId = qualificationId,
                NumberOfItems = numberOfItems,
                Postcode = postcode
            };

            var locations = new List<Location>
            {
                new Location(),
                new Location(),
                new Location()
            }.AsQueryable();
            _locationService.GetLocations(Arg.Is<IQueryable<Provider>>(p => p == providers)).Returns(locations);

            var providerLocations = new List<ProviderLocation>
            {
                new ProviderLocation(),
                new ProviderLocation(),
                new ProviderLocation()
            }.AsQueryable();

            _providerLocationService.GetProviderLocations(Arg.Is<IQueryable<Location>>(l => l == locations), Arg.Is<IQueryable<Provider>>(p => p == providers)).Returns(providerLocations);

            _distanceCalculationService.CalculateProviderLocationDistanceInMiles(
                    Arg.Is<PostcodeLocation>(p => p.Postcode == searchRequest.Postcode),
                    providerLocations)
                .Returns(providerLocations.ToList());

            // Act
            var (totalCount, searchResults) = await _service.Search(searchRequest);

            // Assert
            totalCount.Should().Be(providerLocations.Count());
            searchResults.Count().Should().Be(numberOfItems);
            _providerDataService.Received(1).GetProviders();
            _locationService.Received(1).GetLocations(Arg.Is<IQueryable<Provider>>(p => p == providers));
            _providerLocationService.Received(1).GetProviderLocations(Arg.Is<IQueryable<Location>>(l => l == locations), Arg.Is<IQueryable<Provider>>(p => p == providers));
            await _distanceCalculationService.Received(1).CalculateProviderLocationDistanceInMiles(
                Arg.Is<PostcodeLocation>(p => p.Postcode == searchRequest.Postcode),
                providerLocations);
        }
    }
}