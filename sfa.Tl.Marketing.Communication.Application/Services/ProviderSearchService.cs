using System;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class ProviderSearchService : IProviderSearchService
    {
        private readonly IProviderDataService _providerDataService;
        private readonly IProviderLocationService _providerLocationService;
        private readonly IJourneyService _journeyService;
        private readonly ILocationService _locationService;
        private readonly IDistanceCalculationService _distanceCalculationService;

        public ProviderSearchService(
            IProviderDataService providerDataService, 
            IJourneyService journeyService, 
            ILocationService locationService, 
            IProviderLocationService providerLocationService, 
            IDistanceCalculationService distanceCalculationService)
        {
            _providerDataService = providerDataService;
            _providerLocationService = providerLocationService;
            _journeyService = journeyService;
            _locationService = locationService;
            _distanceCalculationService = distanceCalculationService;
        }

        public IEnumerable<Qualification> GetQualifications()
        {
            var qualifications = _providerDataService.GetQualifications().ToList();
            return qualifications.OrderBy(q => q.Name);
        }

        public IEnumerable<ProviderLocation> GetAllProviderLocations()
        {
            var providers = _providerDataService.GetProviders();
            var locations = _locationService.GetLocations(providers);
            var providerLocations = _providerLocationService.GetProviderLocations(locations, providers);
            return providerLocations;
        }

        public async Task<(int totalCount, IEnumerable<ProviderLocation> searchResults)> Search(SearchRequest searchRequest)
        {
            var providers = _providerDataService.GetProviders();

            var results = new List<ProviderLocation>();

            if (providers.Any())
            {
                IQueryable<Location> locations;
                if (searchRequest.QualificationId.HasValue && searchRequest.QualificationId.Value > 0)
                {
                    locations = _locationService.GetLocations(providers, searchRequest.QualificationId.Value);
                }
                else
                {
                    locations = _locationService.GetLocations(providers);
                }

                var providerLocations = _providerLocationService.GetProviderLocations(locations, providers);

                results = await _distanceCalculationService.CalculateProviderLocationDistanceInMiles(
                    new PostcodeLocation
                    {
                        Postcode = searchRequest.Postcode,
                        Latitude = searchRequest.OriginLatitude,
                        Longitude = searchRequest.OriginLongitude
                    }, providerLocations);
            }

            var totalCount = results.Count;
            var searchResults = results.OrderBy(pl => pl.DistanceInMiles).Take(searchRequest.NumberOfItems);

            var searchOriginLatitude = Convert.ToDouble(searchRequest.OriginLatitude);
            var searchOriginLongitude = Convert.ToDouble(searchRequest.OriginLongitude);

            foreach (var searchResult in searchResults)
            {
                searchResult.JourneyUrl = _journeyService.GetDirectionsLink(
                    searchRequest.Postcode,
                    searchResult);
            }

            return (totalCount, searchResults);
        }

        public Qualification GetQualificationById(int id)
        {
            return _providerDataService.GetQualification(id);
        }

        public async Task<(bool IsValid, PostcodeLocation PostcodeLocation)> IsSearchPostcodeValid(string postcode)
        {
            return await _distanceCalculationService.IsPostcodeValid(postcode);
        }
    }
}
