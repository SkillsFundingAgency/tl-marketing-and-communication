using System;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Diagnostics;
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
            var qualifications = _providerDataService.GetQualifications().ToList();
            return qualifications.OrderBy(q => q.Name);
        }

        public async Task<(int totalCount, IEnumerable<ProviderLocation> searchResults)> Search(SearchRequest searchRequest)
        {
            _logger.LogInformation($"Search::requested search for {searchRequest.Postcode} with {searchRequest.NumberOfItems} for qualification {searchRequest.QualificationId}");

            var stopwatch = Stopwatch.StartNew();   

            var providers = _providerDataService.GetProviders();

            stopwatch.Stop();
            _logger.LogInformation($"Search::Get {providers.Count()} providers took {stopwatch.ElapsedMilliseconds}ms {stopwatch.ElapsedTicks} ticks");
            stopwatch.Restart();

            if (!providers.Any())
            {
                return (0, new List<ProviderLocation>());
            }

            var locations = _providerDataService.GetLocations(providers, searchRequest.QualificationId);
            stopwatch.Stop();
            _logger.LogInformation($"Search::Get {locations.Count()} locations took {stopwatch.ElapsedMilliseconds}ms {stopwatch.ElapsedTicks} ticks");
            stopwatch.Restart();

            var providerLocations = _providerDataService.GetProviderLocations(locations, providers);
            stopwatch.Stop();
            _logger.LogInformation($"Search::Get {providerLocations.Count()} providerLocations took {stopwatch.ElapsedMilliseconds}ms {stopwatch.ElapsedTicks} ticks");
            stopwatch.Restart();

            var providerLocationsWithDistances = await _distanceCalculationService.CalculateProviderLocationDistanceInMiles(
                new PostcodeLocation
                {
                    Postcode = searchRequest.Postcode,
                    Latitude = Convert.ToDouble(searchRequest.OriginLatitude),
                    Longitude = Convert.ToDouble(searchRequest.OriginLongitude)
                }, providerLocations);

            stopwatch.Stop();
            _logger.LogInformation($"Search::Calculate distance for {providerLocations.Count()} providerLocations took {stopwatch.ElapsedMilliseconds}ms {stopwatch.ElapsedTicks} ticks");
            stopwatch.Restart();
            
            var searchResults = providerLocationsWithDistances
                .OrderBy(pl => pl.DistanceInMiles)
                .Take(searchRequest.NumberOfItems)
                .ToList();

            stopwatch.Stop();
            _logger.LogInformation($"Search::Get {searchResults.Count} searchResults took {stopwatch.ElapsedMilliseconds}ms {stopwatch.ElapsedTicks} ticks");
            stopwatch.Restart();
            searchResults.ForEach(s => 
                s.JourneyUrl = _journeyService.GetDirectionsLink(s.Postcode, s));

            stopwatch.Stop();
            _logger.LogInformation($"Search::Set journey url for {searchResults.Count} results took {stopwatch.ElapsedMilliseconds}ms {stopwatch.ElapsedTicks} ticks");

            return (providerLocationsWithDistances.Count(), searchResults);
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
