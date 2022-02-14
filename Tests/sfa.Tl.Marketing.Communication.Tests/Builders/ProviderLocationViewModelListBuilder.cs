using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    public class ProviderLocationViewModelListBuilder
    {
        private readonly IList<ProviderLocationViewModel> _providerLocationViewModels = new List<ProviderLocationViewModel>();
        
        public IList<ProviderLocationViewModel> Build() =>
            _providerLocationViewModels;

        public ProviderLocationViewModelListBuilder Add(int numberOfProviderLocations = 1)
        {
            var deliveryYearViewModelListBuilder = new DeliveryYearViewModelListBuilder();
            var start = _providerLocationViewModels.Count;


            for (var i = 0; i < numberOfProviderLocations; i++)
            {
                var nextId = start + i + 1;
                _providerLocationViewModels.Add(new ProviderLocationViewModel
                {
                    ProviderName = $"Test Provider {nextId}",
                    Name = $"Test Location {nextId}",
                    Postcode = $"CV{nextId} {nextId + 1}WT",
                    Town = "Coventry",
                    Latitude = 52.400997 + i,
                    Longitude = -0.508122 - i,
                    DistanceInMiles = 9 + i,
                    DeliveryYears = deliveryYearViewModelListBuilder.Build(),
                    Website = "https://test.provider.co.uk"
                });
            }

            return this;
        }
    }
}
