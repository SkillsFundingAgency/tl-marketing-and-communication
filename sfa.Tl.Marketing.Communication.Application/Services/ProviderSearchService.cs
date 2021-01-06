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
        private readonly ILocationService _locationService;
        private readonly IDistanceCalculationService _distanceCalculationService;

        public ProviderSearchService(IProviderDataService providerDataService, ILocationService locationService, IProviderLocationService providerLocationService, IDistanceCalculationService distanceCalculationService)
        {
            _providerDataService = providerDataService;
            _providerLocationService = providerLocationService;
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

            searchResults = searchResults.Select(s =>
            {
                s.JourneyUrl = "https://www.google.com/maps/dir/B91+1NG,+Solihull/Solihull+College+%26+University+Centre+Blossomfield+Campus,+Solihull/@52.4113588,-1.795049,17z/data=!3m1!4b1!4m14!4m13!1m5!1m1!1s0x4870b9e9b63f91ef:0x2a0e6b03104f3776!2m2!1d-1.7921997!2d52.4131839!1m5!1m1!1s0x4870b9c249edef4d:0xa9adeba9e68f74c!2m2!1d-1.7924987!2d52.4092323!3e3";
                 return s;
            });

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
