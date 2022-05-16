using System;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace sfa.Tl.Marketing.Communication.Application.Services;

public class ProviderSearchService : IProviderSearchService
{
    private readonly IProviderDataService _providerDataService;
    private readonly IJourneyService _journeyService;
    private readonly IDistanceCalculationService _distanceCalculationService;
    private readonly ILogger<ProviderSearchService> _logger;

    private const string PostcodeRegexPattern =
        "^(([A-Z][0-9]{1,2})|(([A-Z][A-HJ-Y][0-9]{1,2})|(([A-Z][0-9][A-Z])|([A-Z][A-HJ-Y][0-9]?[A-Z]))))( *[0-9][A-Z]{2})?$";

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
            .OrderBy(q => q.Id > 0 ? q.Name : "")
            .Prepend(new Qualification { Id = 0, Name = "All T Level courses" });
    }

    public async Task<(int totalCount, IEnumerable<ProviderLocation> searchResults)> Search(
        SearchRequest searchRequest)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(
                "Search::requested search for {postcode} with {numberOfItems} for qualification {qualificationId}",
                searchRequest.Postcode, searchRequest.NumberOfItems, searchRequest.QualificationId);
        }

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
        if (!Regex.IsMatch(postcode, PostcodeRegexPattern, RegexOptions.IgnoreCase))
        {
            _logger.LogInformation("Postcode regex failed for {postcode}", postcode);
            return (false, null);
        }

        return await _distanceCalculationService.IsPostcodeValid(postcode);
    }
}