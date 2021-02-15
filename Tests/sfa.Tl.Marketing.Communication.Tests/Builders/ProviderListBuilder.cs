using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    public class ProviderListBuilder
    {
        private readonly IList<Provider> _providers = new List<Provider>();

        public IList<Provider> Build() =>
            _providers;

        public ProviderListBuilder Add(int numberOfProviders = 1)
        {
            var start = _providers.Count;
            for (var i = 0; i < numberOfProviders; i++)
            {
                var nextId = start + i + 1;
                _providers.Add(new Provider
                {
                    Id = 10000000 + nextId,
                    UkPrn = 10000000 + nextId,
                    Name = $"Test Provider {nextId}",
                    Locations = new List<Location>
                    {
                        new Location
                        {
                            Postcode = $"CV{nextId} {nextId + 1}WT",
                            Town = "Coventry",
                            Latitude = 50.1234 + nextId,
                            Longitude = -0.234 - nextId,
                            DeliveryYears = new List<DeliveryYearDto>
                            {
                                new DeliveryYearDto
                                {
                                    Year = (short)(2020 + nextId),
                                    Qualifications = new List<int>
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

