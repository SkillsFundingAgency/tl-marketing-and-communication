using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services
{
    public class ProviderSearchServiceUnitTests
    {
        private readonly IProviderDataService _providerDataService;
        private readonly IJourneyService _journeyService;
        private readonly IDistanceCalculationService _distanceCalculationService;
        private readonly IProviderSearchService _service;

        public ProviderSearchServiceUnitTests()
        {
            _providerDataService = Substitute.For<IProviderDataService>();
            _journeyService = Substitute.For<IJourneyService>();
            _distanceCalculationService = Substitute.For<IDistanceCalculationService>();
            var logger = Substitute.For<ILogger<ProviderSearchService>>();

            _service = new ProviderSearchService(
                _providerDataService,
                _journeyService,
                _distanceCalculationService,
                logger);
        }

        [Fact]
        public void GetQualifications_Returns_All_Qualifications_OrderBy_Name()
        {
            var qualifications = new List<Qualification>
            {
                new() { Id = 0, Name = "All T Level courses" },
                new() { Id = 1, Name = "Xyz" },
                new() { Id = 2, Name = "Mno" },
                new() { Id = 3, Name = "Abc" }
            };

            _providerDataService.GetQualifications()
                .Returns(qualifications);

            var expected = new List<Qualification>
            {
                qualifications.Single(q => q.Id == 0),
                qualifications.Single(q => q.Id == 3),
                qualifications.Single(q => q.Id == 2),
                qualifications.Single(q => q.Id == 1)
            };

            var actual = _service.GetQualifications().ToList();

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i].Id.Should().Be(expected[i].Id);
                actual[i].Name.Should().Be(expected[i].Name);
            }

            Assert.True(actual.SequenceEqual(expected));
            Assert.False(actual.SequenceEqual(qualifications));
            _providerDataService.Received(1).GetQualifications();
        }

        [Fact]
        public void GetQualificationById_Returns_A_Qualification()
        {
            const int id = 1;
            const string name = "Test Qualification";

            var expected = new Qualification { Id = id, Name = name };

            _providerDataService.GetQualification(id).Returns(expected);

            var actual = _service.GetQualificationById(id);

            actual.Should().BeSameAs(expected);
            _providerDataService.Received(1).GetQualification(id);
        }

        [Theory]
        [InlineData("mk128jk", true)]
        [InlineData("mk980kl", false)]
        public async Task IsSearchPostcodeValid_Validate_A_Postcode(string postcode, bool expected)
        {
            _distanceCalculationService.IsPostcodeValid(postcode).Returns((expected, new PostcodeLocation { Postcode = postcode }));

            var (isValid, postcodeLocation) = await _service.IsSearchPostcodeValid(postcode);

            isValid.Should().Be(expected);
            postcodeLocation.Should().NotBeNull();
            postcodeLocation.Postcode.Should().Be(postcode);
            await _distanceCalculationService.Received(1).IsPostcodeValid(postcode);
        }

        [Fact]
        public async Task Search_Returns_Empty_Providers_And_TotalRecordCount()
        {
            _providerDataService.GetProviders().Returns(new List<Provider>().AsQueryable());

            var (totalCount, searchResults) = await _service.Search(new SearchRequest());

            totalCount.Should().Be(0);
            searchResults.Count().Should().Be(0);
            _providerDataService.Received(1).GetProviders();
            _journeyService.DidNotReceive()
                .GetDirectionsLink(Arg.Any<string>(), Arg.Any<ProviderLocation>());
        }


        [Fact]
        public async Task Search_Returns_ProviderLocations_With_Expected_Details()
        {
            var providers = new ProviderListBuilder()
                .Add()
                .Build()
                .AsQueryable();

            _providerDataService.GetProviders().Returns(providers);

            const int numberOfItems = 1;
            const string postcode = "CV1 2WT";
            var searchRequest = new SearchRequest
            {
                NumberOfItems = numberOfItems,
                Postcode = postcode,
                OriginLatitude = "1.5",
                OriginLongitude = "50"
            };

            var locations = new List<Location>
            {
                providers.First().Locations.First()
            }.AsQueryable();

            _providerDataService.GetLocations(
                Arg.Is<IQueryable<Provider>>(p => p == providers),
                Arg.Any<int?>())
                .Returns(locations);

            var deliveryYears = new List<DeliveryYear>
            {
                new()
                {
                    Year = providers.First().Locations.First().DeliveryYears.First().Year,
                    Qualifications = new List<Qualification>()
                    {
                        new()
                        {
                            Id = providers.First().Locations.First().DeliveryYears.First()
                                .Qualifications.First(),
                            Name = "Test qualification"
                        }
                    }
                }
            };

            var providerLocations = new List<ProviderLocation>
            {
                new()
                {
                    ProviderName = providers.First().Name,
                    Name  = providers.First().Locations.First().Name,
                    Postcode  = providers.First().Locations.First().Postcode,
                    Town = providers.First().Locations.First().Town,
                    Latitude = providers.First().Locations.First().Latitude,
                    Longitude = providers.First().Locations.First().Longitude,
                    DistanceInMiles = 0,
                    DeliveryYears = deliveryYears,
                    Website  = providers.First().Locations.First().Website,
                    JourneyUrl = ""
                }
            }.AsQueryable();

            _providerDataService.GetProviderLocations(
                    Arg.Is<IQueryable<Location>>(l => l == locations),
                    Arg.Is<IQueryable<Provider>>(p => p == providers))
                .Returns(providerLocations);

            _distanceCalculationService.CalculateProviderLocationDistanceInMiles(
                Arg.Is<PostcodeLocation>(p => p.Postcode == searchRequest.Postcode),
                Arg.Any<IQueryable<ProviderLocation>>())
                .Returns(args =>
                {
                    var pList = ((IEnumerable<ProviderLocation>)args[1]).ToList();
                    pList.ForEach(x => x.DistanceInMiles = 10);
                    return pList;
                });

            _journeyService
                .GetDirectionsLink(searchRequest.Postcode, Arg.Any<ProviderLocation>())
                .Returns("https://x.com");

            var (totalCount, searchResults) =
                await _service.Search(searchRequest);

            totalCount.Should().Be(providerLocations.Count());
            var searchResultsList = searchResults.ToList();
            searchResultsList.Count.Should().Be(numberOfItems);

            var firstResult = searchResultsList.First();
            firstResult.ProviderName.Should().Be(providers.First().Name);
            firstResult.Name.Should().Be(providers.First().Locations.First().Name);
            firstResult.Postcode.Should().Be(providers.First().Locations.First().Postcode);
            firstResult.Town.Should().Be(providers.First().Locations.First().Town);
            firstResult.Latitude.Should().Be(providers.First().Locations.First().Latitude);
            firstResult.Longitude.Should().Be(providers.First().Locations.First().Longitude);
            firstResult.Website.Should().Be(providers.First().Locations.First().Website);

            firstResult.DeliveryYears.Should().HaveCount(1);
            firstResult.DeliveryYears.First().Year.Should()
                .Be(providers.First().Locations.First().DeliveryYears.First().Year);

            firstResult.DeliveryYears.First().Qualifications.Should().HaveCount(1);
            firstResult.DeliveryYears.First().Qualifications.First().Id.Should()
                .Be(providers.First().Locations.First().DeliveryYears.First().Qualifications.First());
            firstResult.DeliveryYears.First().Qualifications.First().Name.Should()
                .Be("Test qualification");

            firstResult.JourneyUrl.Should().Be("https://x.com");

            firstResult.DistanceInMiles.Should().Be(10);
        }

        [Fact]
        public async Task Search_Returns_ProviderLocations_Filtered_By_Qualification_And_NumberOfItems()
        {
            var providers = new List<Provider>
            {
                new(),
                new(),
                new()
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
                new(),
                new(),
                new()
            }.AsQueryable();
            _providerDataService.GetLocations(Arg.Is<IQueryable<Provider>>(p => p == providers), Arg.Is<int>(q => q == searchRequest.QualificationId.Value)).Returns(locations);

            var providerLocations = new List<ProviderLocation>
            {
                new(),
                new(),
                new()
            }.AsQueryable();

            _providerDataService.GetProviderLocations(
                    Arg.Is<IQueryable<Location>>(l => l == locations),
                    Arg.Is<IQueryable<Provider>>(p => p == providers))
                .Returns(providerLocations);

            _distanceCalculationService.CalculateProviderLocationDistanceInMiles(
                    Arg.Is<PostcodeLocation>(p => p.Postcode == searchRequest.Postcode
                                                  && p.Latitude.ToString() == searchRequest.OriginLatitude
                                                  && p.Longitude.ToString() == searchRequest.OriginLongitude),
                    providerLocations)
                .Returns(providerLocations.ToList());

            var (totalCount, searchResults) = await _service.Search(searchRequest);

            totalCount.Should().Be(providerLocations.Count());
            searchResults.Count().Should().Be(numberOfItems);
            _providerDataService.Received(1).GetProviders();
            _providerDataService.Received(1).GetLocations(Arg.Is<IQueryable<Provider>>(p => p == providers), Arg.Is<int>(q => q == searchRequest.QualificationId.Value));
            _providerDataService.Received(1).GetProviderLocations(Arg.Is<IQueryable<Location>>(l => l == locations), Arg.Is<IQueryable<Provider>>(p => p == providers));
            await _distanceCalculationService.Received(1).CalculateProviderLocationDistanceInMiles(
                Arg.Is<PostcodeLocation>(p => p.Postcode == searchRequest.Postcode),
                providerLocations);
            _journeyService.Received(numberOfItems)
                .GetDirectionsLink(searchRequest.Postcode, Arg.Any<ProviderLocation>());
        }

        [Theory]
        [InlineData(null, 1)]
        [InlineData(0, 2)]
        public async Task Search_Returns_All_ProviderLocations_When_Qualification_Filter_Is_Null_Or_Zero(int? qualificationId, int numberOfItems)
        {
            var providers = new List<Provider>
            {
                new(),
                new(),
                new()
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
                new(),
                new(),
                new()
            }.AsQueryable();

            _providerDataService.GetLocations(Arg.Is<IQueryable<Provider>>(p => p == providers), qualificationId).Returns(locations);

            var providerLocations = new List<ProviderLocation>
            {
                new(),
                new(),
                new()
            }.AsQueryable();

            _providerDataService.GetProviderLocations(
                Arg.Is<IQueryable<Location>>(l => l == locations),
                Arg.Is<IQueryable<Provider>>(p => p == providers))
                .Returns(providerLocations);

            _distanceCalculationService.CalculateProviderLocationDistanceInMiles(
                    Arg.Is<PostcodeLocation>(p => p.Postcode == searchRequest.Postcode),
                    providerLocations)
                .Returns(providerLocations.ToList());

            var (totalCount, searchResults) = await _service.Search(searchRequest);

            totalCount.Should().Be(providerLocations.Count());
            searchResults.Count().Should().Be(numberOfItems);
            _providerDataService.Received(1).GetProviders();
            _providerDataService.Received(1).GetLocations(Arg.Is<IQueryable<Provider>>(p => p == providers), qualificationId);
            _providerDataService.Received(1).GetProviderLocations(Arg.Is<IQueryable<Location>>(l => l == locations), Arg.Is<IQueryable<Provider>>(p => p == providers));
            await _distanceCalculationService.Received(1).CalculateProviderLocationDistanceInMiles(
                Arg.Is<PostcodeLocation>(p => p.Postcode == searchRequest.Postcode),
                providerLocations);
            _journeyService.Received(numberOfItems)
                .GetDirectionsLink(searchRequest.Postcode, Arg.Any<ProviderLocation>());
        }
    }
}
