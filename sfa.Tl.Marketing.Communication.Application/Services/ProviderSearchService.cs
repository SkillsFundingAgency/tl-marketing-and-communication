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
        private readonly IJourneyService _journeyService;
        private readonly IDistanceCalculationService _distanceCalculationService;

        public ProviderSearchService(
            IProviderDataService providerDataService, 
            IJourneyService journeyService, 
            IDistanceCalculationService distanceCalculationService)
        {
            _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
            _journeyService = journeyService ?? throw new ArgumentNullException(nameof(journeyService));
            _distanceCalculationService = distanceCalculationService ?? throw new ArgumentNullException(nameof(distanceCalculationService));
        }

        public IEnumerable<Qualification> GetQualifications()
        {
            var qualifications = _providerDataService.GetQualifications().ToList();
            return qualifications.OrderBy(q => q.Name);
        }

        public IEnumerable<ProviderLocation> GetAllProviderLocations()
        {
            var providers = _providerDataService.GetProviders();
            var locations = _providerDataService.GetLocations(providers);
            return _providerDataService.GetProviderLocations(locations, providers);
        }

        public async Task<(int totalCount, IEnumerable<ProviderLocation> searchResults)> Search(SearchRequest searchRequest)
        {
            var providers = _providerDataService.GetProviders();

            var results = new List<ProviderLocation>();

            if (providers.Any())
            {
                var locations = _providerDataService.GetLocations(providers, searchRequest.QualificationId);

                var providerLocations = _providerDataService.GetProviderLocations(locations, providers);

                results = await _distanceCalculationService.CalculateProviderLocationDistanceInMiles(
                    new PostcodeLocation
                    {
                        Postcode = searchRequest.Postcode,
                        Latitude = Convert.ToDouble(searchRequest.OriginLatitude),
                        Longitude = Convert.ToDouble(searchRequest.OriginLongitude)
                    }, providerLocations);
            }

            var totalCount = results.Count;
            var searchResults = results
                .OrderBy(pl => pl.DistanceInMiles)
                .Take(searchRequest.NumberOfItems)
                .ToList();

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
