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
                            Website = $"https://test.provider_{nextId}.co.uk",
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
                            }
                        }
                    }
                });
            }

            return this;
        }

        public ProviderListBuilder CreateKnownList()
        {
            _providers.Clear();
            _providers.Add(new Provider
            {
                UkPrn = 10000001,
                Name = "TEST COLLEGE TO BE DELETED",
                Locations = new List<Location>
                {
                    new Location
                    {
                        Postcode = "CV1 2WT",
                        Town = "Coventry",
                        Latitude = 52.400997,
                        Longitude = -1.508122,
                        Website = "http://www.test.co.uk",
                        DeliveryYears = new List<DeliveryYearDto>()
                    }
                }
            });
            _providers.Add(new Provider
            {
                UkPrn = 10000055,
                Name = "ABINGDON AND WITNEY COLLEGE",
                Locations = new List<Location>
                {
                    new Location
                    {
                        Postcode = "OX14 1GG",
                        Name = "ABINGDON CAMPUS",
                        Town = "Abingdon",
                        Latitude = 51.680637,
                        Longitude = -1.286943,
                        Website = "http://www.abingdon-witney.ac.uk",
                        DeliveryYears = new List<DeliveryYearDto>
                        {
                            new DeliveryYearDto
                            {
                                Year = 2021,
                                Qualifications = new List<int>
                                {
                                    36
                                }
                            }
                        }
                    }
                }
            });
            _providers.Add(new Provider
            {
                UkPrn = 10004375,
                Name = "MILTON KEYNES COLLEGE",
                Locations = new List<Location>
                {
                    new Location
                    {
                        Postcode = "MK3 6DR",
                        Town = "Milton Keynes",
                        Name = "Bletchley Campus",
                        Latitude = 51.995631,
                        Longitude = -0.737973,
                        Website = "http://www.provider.com/tlevel",
                        DeliveryYears = new List<DeliveryYearDto>
                        {
                            new DeliveryYearDto
                            {
                                Year = 2021,
                                Qualifications = new List<int>
                                {
                                    40
                                }
                            }
                        }
                    }
                }
            });
            _providers.Add(new Provider
            {
                UkPrn = 10033240,
                Name = "RUGBY HIGH SCHOOL",
                Locations = new List<Location>
                {
                    new Location
                    {
                        Postcode = "CV22 7RE",
                        Name = "Rugby High School",
                        Town = "Rugby",
                        Latitude = 52.352711,
                        Longitude = -1.28768,
                        Website = "http://www.rugbyhighschool.co.uk",
                        DeliveryYears = new List<DeliveryYearDto>
                        {
                            new DeliveryYearDto
                            {
                                Year = 2021,
                                Qualifications = new List<int>
                                {
                                    36,
                                    37,
                                    42,
                                    43,
                                    44
                                }
                            },
                            new DeliveryYearDto
                            {
                                Year = 2022,
                                Qualifications = new List<int>
                                {
                                    38
                                }
                            }
                        }
                    },
                    new Location
                    {
                        Postcode = "CV22 5ET",
                        Name = "TEST",
                        Town = "Rugby",
                        Latitude = 52.357811,
                        Longitude = -1.228056,
                        Website = "http://www.rhswebsite.co.uk",
                        DeliveryYears = new List<DeliveryYearDto>
                        {
                            new DeliveryYearDto
                            {
                                Year = 2021,
                                Qualifications = new List<int>
                                {
                                    37,
                                    42,
                                    43
                                }
                            }
                        }
                    }
                }
            });

            return this;
        }
    }
}

