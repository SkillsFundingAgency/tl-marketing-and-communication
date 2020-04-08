using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class ProviderSearchService : IProviderSearchService
    {
        private readonly IProviderService _providerService;
        private readonly IProviderLocationService _providerLocationService;
        private readonly ILocationService _locationService;
        private readonly IDistanceCalculationService _distanceCalculationService;

        private IQueryable<Provider> _providers;

        public ProviderSearchService(IProviderService providerService, ILocationService locationService, IProviderLocationService providerLocationService, IDistanceCalculationService distanceCalculationService)
        {
            _providerService = providerService;
            _providerLocationService = providerLocationService;
            _locationService = locationService;
            _distanceCalculationService = distanceCalculationService;
        }

        public async Task<IEnumerable<ProviderLocation>> Search(SearchRequest searchRequest)
        {
            _providers = _providerService.GetProviders();

            IQueryable<Location> locations = new List<Location>().AsQueryable();
            IQueryable<ProviderLocation> providerLocations = new List<ProviderLocation>().AsQueryable();

            if (_providers.Any())
            {
                if (searchRequest.QualificationId.HasValue && searchRequest.QualificationId.Value > 0)
                {
                    locations = _locationService.GetLocations(_providers, searchRequest.QualificationId.Value);
                }
                else
                {
                    locations = _locationService.GetLocations(_providers);
                }

                providerLocations = _providerLocationService.GetProviderLocations(locations);

                await _distanceCalculationService.CalculateProviderLocationDistanceInMiles(searchRequest.Postcode, providerLocations);
            }

            return providerLocations.OrderBy(pl => pl.DistanceInMiles).Take(searchRequest.NumberOfItems);
        }
    }
}
