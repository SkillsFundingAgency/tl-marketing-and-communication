using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    public class ProviderLocationListBuilder
    {
        private readonly IList<ProviderLocation> _providerLocations = new List<ProviderLocation>();
        
        public IList<ProviderLocation> Build() =>
            _providerLocations;

        public ProviderLocationListBuilder Add(int numberOfProviderLocations = 1)
        {
            var start = _providerLocations.Count;
            for (var i = 0; i < numberOfProviderLocations; i++)
            {
                var nextId = start + i + 1;
                _providerLocations.Add(new ProviderLocation
                {
                    ProviderName = $"Test Provider {nextId}",
                    Name = $"Test Location {nextId}",
                    Postcode = $"CV{nextId} {nextId + 1}WT",
                    Town = "Coventry",
                    Latitude = 52.400997 + i,
                    Longitude = -1.508122 - i,
                    DistanceInMiles = 9.5 + i,
                    DeliveryYears = new List<DeliveryYear>
                    {
                        new DeliveryYear
                        {
                            Year = (short)(2020 + nextId),
                            Qualifications = new List<Qualification>
                            {
                                new Qualification
                                {
                                    Id = nextId,
                                    Name = $"Qualification {nextId}"
                                }
                            }

                        }
                    },
                    Website = "https://test.provider.co.uk"
                });
            }

            return this;
        }
    }
}
