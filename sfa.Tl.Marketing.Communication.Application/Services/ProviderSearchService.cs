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

        public async Task<(int totalCount, IEnumerable<ProviderLocation> searchResults)> Search(SearchRequest searchRequest)
        {
            var providers = _providerDataService.GetProviders();

            IQueryable<Location> locations = new List<Location>().AsQueryable();
            var results = new List<ProviderLocation>();

            if (providers.Any())
            {
                if (searchRequest.QualificationId.HasValue && searchRequest.QualificationId.Value > 0)
                {
                    locations = _locationService.GetLocations(providers, searchRequest.QualificationId.Value);
                }
                else
                {
                    locations = _locationService.GetLocations(providers);
                }

                var providerLocations = _providerLocationService.GetProviderLocations(locations, providers);

                results = await _distanceCalculationService.CalculateProviderLocationDistanceInMiles(searchRequest.Postcode, providerLocations);
            }

            var totalCount = results.Count();
            var searchResults = results.OrderBy(pl => pl.DistanceInMiles).Take(searchRequest.NumberOfItems);

            return (totalCount, searchResults);
        }

        public Qualification GetQualificationById(int id)
        {
            var qualification = _providerDataService.GetQualification(id);
            return qualification;
        }

        public async Task<(bool IsValid, string Postcode)> IsSearchPostcodeValid(string postcode)
        {
            var result = await _distanceCalculationService.IsPostcodeValid(postcode);
            return result;
        }
    }
}
