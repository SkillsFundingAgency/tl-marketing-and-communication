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

        public async Task<IEnumerable<ProviderLocation>> Search(SearchRequest searchRequest)
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

            return results.OrderBy(pl => pl.DistanceInMiles).Take(searchRequest.NumberOfItems);
        }
    }
}
