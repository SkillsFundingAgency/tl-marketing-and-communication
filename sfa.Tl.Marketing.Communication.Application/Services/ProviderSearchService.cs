using System;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class ProviderSearchService : IProviderSearchService
    {
        private readonly IProviderDataService _providerDataService;
        private readonly IJourneyService _journeyService;
        private readonly IDistanceCalculationService _distanceCalculationService;
        private readonly ILogger<ProviderSearchService> _logger;

        public ProviderSearchService(
            IProviderDataService providerDataService,
            IJourneyService journeyService,
            IDistanceCalculationService distanceCalculationService,
            ILogger<ProviderSearchService> logger)
        {
            _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
            _journeyService = journeyService ?? throw new ArgumentNullException(nameof(journeyService));
            _distanceCalculationService = distanceCalculationService ?? throw new ArgumentNullException(nameof(distanceCalculationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<Qualification> GetQualifications()
        {
            return _providerDataService
                .GetQualifications()
                .OrderBy(q => q.Id > 0 ? q.Name : "");
        }

        public async Task<(int totalCount, IEnumerable<ProviderLocation> searchResults)> Search(
            SearchRequest searchRequest)
        {
            _logger.LogInformation($"Search::requested search for {searchRequest.Postcode} with {searchRequest.NumberOfItems} for qualification {searchRequest.QualificationId}");

            var providerLocations = _providerDataService
                .GetProviderLocations(searchRequest.QualificationId);

            var providerLocationsWithDistances = await _distanceCalculationService.CalculateProviderLocationDistanceInMiles(
                new PostcodeLocation
                {
                    Postcode = searchRequest.Postcode,
                    Latitude = Convert.ToDouble(searchRequest.OriginLatitude),
                    Longitude = Convert.ToDouble(searchRequest.OriginLongitude)
                }, providerLocations);

            var searchResults = providerLocationsWithDistances
                .OrderBy(pl => pl.DistanceInMiles)
                .Take(searchRequest.NumberOfItems)
                .Select(s =>
                {
                    s.JourneyUrl = _journeyService.GetDirectionsLink(searchRequest.Postcode, s);
                    return s;
                });

            return (providerLocationsWithDistances.Count, searchResults);
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
