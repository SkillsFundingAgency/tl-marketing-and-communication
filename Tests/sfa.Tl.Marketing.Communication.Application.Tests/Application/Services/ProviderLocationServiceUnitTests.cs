using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Dto;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services
{
    public class ProviderLocationServiceUnitTests
    {
        private readonly IProviderDataService _providerDataService;
        private readonly IProviderLocationService _service;

        public ProviderLocationServiceUnitTests()
        {
            _providerDataService = Substitute.For<IProviderDataService>();

            _service = new ProviderLocationService(_providerDataService);
        }

        [Fact]
        public void GetProviderLocations_Returns_Expected_Results_For_Multiple_Providers()
        {
            var qualifications = BuildQualifications();

            var location1 = BuildLocation("Location 1", "CV1 2WT", 52.345, -2.001);
            location1.DeliveryYears.Add(new DeliveryYearDto
            {
                Year = 2020,
                Qualifications = new List<int> { 1, 2 }
            });
            location1.DeliveryYears.Add(new DeliveryYearDto
            {
                Year = 2021,
                Qualifications = new List<int> { 3 }
            });
            
            var location2 = BuildLocation("Location 2", "S70 2YW", 50.001, -1.234);
            location2.DeliveryYears.Add(new DeliveryYearDto
            {
                Year = 2020,
                Qualifications = new List<int> { 1 }
            });

            var locations = BuildLocations(location1, location2);

            var providers = new List<Provider>
            {
                BuildProvider(1, "Provider 1", new List<Location> { location1 }),
                BuildProvider(2,"Provider 2", new List<Location> { location2 })
            }.AsQueryable();

            _providerDataService.GetQualifications().Returns(qualifications);
            _providerDataService.
                GetQualifications(Arg.Any<int[]>())
                .Returns(x => qualifications.Where(q => ((int[])x[0]).Contains(q.Id)));

            var results = _service.GetProviderLocations(locations, providers).ToList();

            results.Count.Should().Be(locations.Count());
            results.Should().Contain(p =>
                p.ProviderName == "Provider 1" &&
                p.Name == "Location 1" &&
                p.Postcode == "CV1 2WT" &&
                Math.Abs(p.Latitude - 52.345) < 0.001 &&
                Math.Abs(p.Longitude - (-2.001)) < 0.001 &&
                p.DeliveryYears.Count() == 2 &&
                p.DeliveryYears.Count(dy => dy.Year == 2020) == 1 &&
                p.DeliveryYears.Single(dy => dy.Year == 2020).Qualifications
                    .Contains(qualifications.Single(q => q.Id == 1)) &&
                p.DeliveryYears.Single(dy => dy.Year == 2020).Qualifications
                    .Contains(qualifications.Single(q => q.Id == 2)) &&
                p.DeliveryYears.Count(dy => dy.Year == 2021) == 1 &&
                p.DeliveryYears.Single(dy => dy.Year == 2021).Qualifications
                    .Contains(qualifications.Single(q => q.Id == 3)));

            results.Should().Contain(p =>
                p.ProviderName == "Provider 2" &&
                p.Name == "Location 2" &&
                p.Postcode == "S70 2YW" &&
                Math.Abs(p.Latitude - 50.001) < 0.001 &&
                Math.Abs(p.Longitude - (-1.234)) < 0.001 &&
                p.DeliveryYears.Count() == 1 &&
                p.DeliveryYears.Count(dy => dy.Year == 2020) == 1 &&
                p.DeliveryYears.Single(dy => dy.Year == 2020).Qualifications
                    .Contains(qualifications.Single(q => q.Id == 1)));
        }

        [Fact]
        public void GetProviderLocations_Returns__Delivery_Years_In_Order()
        {
            var qualifications = BuildQualifications();

            var location1 = BuildLocation("Location 1", "CV1 2WT", 52.345, -2.001);
            location1.DeliveryYears.Add(new DeliveryYearDto
            {
                Year = 2022,
                Qualifications = new List<int> { 3 }
            });
            location1.DeliveryYears.Add(new DeliveryYearDto
            {
                Year = 2021,
                Qualifications = new List<int> { 1 }
            });
            location1.DeliveryYears.Add(new DeliveryYearDto
            {
                Year = 2020,
                Qualifications = new List<int> { 1 }
            });

            var locations = BuildLocations(location1);

            var providers = new List<Provider>
            {
                BuildProvider(1, "Provider 1", new List<Location> { location1 })
            }.AsQueryable();

            _providerDataService.GetQualifications().Returns(qualifications);
            _providerDataService.
                GetQualifications(Arg.Any<int[]>())
                .Returns(x => qualifications.Where(q => ((int[])x[0]).Contains(q.Id)));

            var results = _service.GetProviderLocations(locations, providers).ToList();
            
            results.Count.Should().Be(locations.Count());
            var deliveryYears = results.First().DeliveryYears.ToList();

            deliveryYears.Count.Should().Be(3);
            deliveryYears[0].Year.Should().Be(2020);
            deliveryYears[1].Year.Should().Be(2021);
            deliveryYears[2].Year.Should().Be(2022);
        }
        
        [Fact]
        public void GetProviderLocations_Returns_Expected_Results_When_One_Location_Has_No_Qualifications()
        {
            var qualifications = BuildQualifications();

            var location1 = BuildLocation("Location 1", "CV1 2WT", 52.345, -2.001);

            var location2 = BuildLocation("Location 2", "S70 2YW", 50.001, -1.234);
            location2.DeliveryYears.Add(new DeliveryYearDto
            {
                Year = 2020,
                Qualifications = new List<int> { 1 }
            });

            var locations = BuildLocations(location1, location2);

            var providers = new List<Provider>
            {
                BuildProvider(1, "Provider 1", new List<Location> { location1 }),
                BuildProvider(2,"Provider 2", new List<Location> { location2 })
            }.AsQueryable();

            _providerDataService.GetQualifications().Returns(qualifications);
            _providerDataService.
                GetQualifications(Arg.Any<int[]>())
                .Returns(x => qualifications.Where(q => ((int[])x[0]).Contains(q.Id)));

            var results = _service.GetProviderLocations(locations, providers).ToList();

            results.Count.Should().Be(2);
            //Fix for bug TLWP-1284 should give the following result:
            //results.Count.Should().Be(1);

            results.Should().Contain(p =>
                    p.ProviderName == "Provider 2" &&
                    p.Name == "Location 2" &&
                    p.Postcode == "S70 2YW" &&
                    Math.Abs(p.Latitude - 50.001) < 0.001 &&
                    Math.Abs(p.Longitude - (-1.234)) < 0.001 &&
                    p.DeliveryYears.Count() == 1 &&
                    p.DeliveryYears.Count(dy => dy.Year == 2020) == 1 &&
                    p.DeliveryYears.Single(dy => dy.Year == 2020).Qualifications
                        .Contains(qualifications.Single(q => q.Id == 1)));
        }

        [Fact]
        public void GetProviderLocations_Returns_Expected_Results_When_No_Locations_Have_Qualifications()
        {
            var qualifications = BuildQualifications();

            var location1 = BuildLocation("Location 1", "CV1 2WT", 52.345, -2.001);
            var location2 = BuildLocation("Location 2", "S70 2YW", 50.001, -1.234);
            var locations = BuildLocations(location1, location2);

            var providers = new List<Provider>
            {
                BuildProvider(1, "Provider 1", new List<Location> { location1 }),
                BuildProvider(2,"Provider 2", new List<Location> { location2 })
            }.AsQueryable();

            _providerDataService.GetQualifications().Returns(qualifications);
            _providerDataService.
                GetQualifications(Arg.Any<int[]>())
                .Returns(x => qualifications.Where(q => ((int[])x[0]).Contains(q.Id)));

            var results = _service.GetProviderLocations(locations, providers).ToList();

            results.Count.Should().Be(2);
            //Fix for bug TLWP-1284 should give the following result:
            //results.Count.Should().Be(0);
        }

        private static Location BuildLocation(string name, string postcode, double lat, double lng)
        {
            return new Location
            {
                Name = name,
                Postcode = postcode,
                Latitude = lat,
                Longitude = lng,
                DeliveryYears = new List<DeliveryYearDto>()
            };
        }

        private static IQueryable<Location> BuildLocations(params Location[] locations)
        {
            return locations.ToList().AsQueryable();
        }

        private static Provider BuildProvider(int id, string name, List<Location> locations)
        {
            return new Provider
            {
                Id = id,
                Name = name,
                Locations = locations
            };
        }

        private static List<Qualification> BuildQualifications()
        {
            var qualifications = new List<Qualification>()
            {
                new Qualification {Id = 1, Name = "Xyz"},
                new Qualification {Id = 2, Name = "Mno"},
                new Qualification {Id = 3, Name = "Abc"}
            };
            return qualifications;
        }
    }
}
