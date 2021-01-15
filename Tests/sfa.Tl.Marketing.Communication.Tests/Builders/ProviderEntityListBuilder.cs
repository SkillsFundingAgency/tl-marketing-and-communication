using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Entities;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    public class ProviderEntityListBuilder
    {
        private readonly IList<ProviderEntity> _providerEntities = new List<ProviderEntity>();

        public IList<ProviderEntity> Build() =>
            _providerEntities;

        public ProviderEntityListBuilder Add(int numberOfProviderEntities = 1)
        {
            var start = _providerEntities.Count;
            for (var i = 0; i < numberOfProviderEntities; i++)
            {
                var nextId = start + i + 1;
                _providerEntities.Add(new ProviderEntity
                {
                    Id = nextId,
                    Name = $"Test Provider {nextId}",
                    Locations = new List<LocationEntity>
                    {
                        new LocationEntity()
                        {
                            Postcode = $"CV{nextId} {nextId + 1}WT",
                            Town = "Coventry",
                            Latitude = 50.1234 + nextId,
                            Longitude = -0.234 - nextId,
                            DeliveryYears = new List<DeliveryYearEntity>
                            {
                                new DeliveryYearEntity
                                {
                                    Year = (short)(2020 + nextId),
                                    Qualifications = new List<int>()
                                    {
                                        nextId
                                    }
                                }
                            },
                            Website = $"https://test.provider_{nextId}.co.uk"
                        }
                    }
                });
            }

            return this;
        }
    }
}
