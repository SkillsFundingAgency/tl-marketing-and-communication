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

        public async Task<(int totalCount, IEnumerable<ProviderLocation> searchResults)> Search(SearchRequest searchRequest)
        {
            /*******************************************************************/
            var new_results = await Search_NEW(searchRequest);
            /*******************************************************************/

            _logger.LogInformation($"Search::requested search for {searchRequest.Postcode} with {searchRequest.NumberOfItems} for qualification {searchRequest.QualificationId}");

            var providers = _providerDataService.GetProviders();
            
            if (!providers.Any())
            {
                return (0, new List<ProviderLocation>());
            }

            var locations = _providerDataService.GetLocations(providers, searchRequest.QualificationId);
            var providerLocations = _providerDataService.GetProviderLocations(locations, providers);
            
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
                })
                .ToList();

            _logger.LogInformation($"Search::Returning {searchResults.Count} results for {searchRequest.Postcode} with {searchRequest.NumberOfItems} items and selected qualification {searchRequest.QualificationId}");

            /*******************************************************************/
            if (new_results.totalCount != providerLocationsWithDistances.Count)
                            throw new Exception("mismatch");
            if (new_results.searchResults.Count() != searchResults.Count)
                            throw new Exception("mismatch");

            for (var i = 0; i < searchResults.Count; i++)
            {
                var result = searchResults.ToList()[i];
                var new_result = new_results.searchResults.ToList()[i];

                if (new_result.Name != result.Name)
                    throw new Exception("mismatch");
                if (new_result.Postcode != result.Postcode)
                    throw new Exception("mismatch");
                if (new_result.Town != result.Town)
                    throw new Exception("mismatch");
                if (new_result.JourneyUrl != result.JourneyUrl)
                    throw new Exception("mismatch");
                if (new_result.ProviderName != result.ProviderName)
                    throw new Exception("mismatch");
                if (new_result.Website != result.Website)
                    throw new Exception("mismatch");
                if (new_result.Latitude != result.Latitude)
                    throw new Exception("mismatch");
                if (new_result.Longitude != result.Longitude)
                    throw new Exception("mismatch");
                if (new_result.DistanceInMiles != result.DistanceInMiles)
                    throw new Exception("mismatch");

                if (result.DeliveryYears is null && new_result.DeliveryYears is not null)
                    throw new Exception("mismatch");
                if (result.DeliveryYears is not null && new_result.DeliveryYears is null)
                    throw new Exception("mismatch");

                if (result.DeliveryYears is null)
                    continue;

                if (new_result.DeliveryYears.Count() != result.DeliveryYears.Count())
                    throw new Exception("mismatch");

                for (var j = 0; j < searchResults[i].DeliveryYears.Count(); j++)
                {
                    var deliveryYear = result.DeliveryYears.ToList()[j];
                    var new_deliveryYear = new_result.DeliveryYears.ToList()[j];

                    if (new_deliveryYear.Year != deliveryYear.Year)
                        throw new Exception("mismatch");
                    if (new_deliveryYear.Qualifications.Count() != deliveryYear.Qualifications.Count())
                        throw new Exception("mismatch");

                    for (var k = 0; k < deliveryYear.Qualifications.Count(); k++)
                    {
                        var qualification = deliveryYear.Qualifications.ToList()[k];
                        var new_qualification = new_deliveryYear.Qualifications.ToList()[k];

                        if (new_qualification.Id != qualification.Id)
                            throw new Exception("mismatch");
                        if (new_qualification.Name != qualification.Name)
                            throw new Exception("mismatch");
                        if (new_qualification.Route != qualification.Route)
                            throw new Exception("mismatch");
                    }
                }
            }
            return (new_results.totalCount, new_results.searchResults);
            /*******************************************************************/

            return (providerLocationsWithDistances.Count, searchResults);
        }

        public async Task<(int totalCount, IEnumerable<ProviderLocation> searchResults)> Search_NEW(SearchRequest searchRequest)
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
                })
                .ToList();

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
