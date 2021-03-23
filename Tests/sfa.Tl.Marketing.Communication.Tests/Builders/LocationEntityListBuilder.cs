using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Entities;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    public class LocationEntityListBuilder
    {
        private readonly IList<LocationEntity> _locationEntities = new List<LocationEntity>();

        public IList<LocationEntity> Build() =>
            _locationEntities;

        public LocationEntityListBuilder Add(int numberOfLocationEntities = 1)
        {
            var start = _locationEntities.Count;
            for (var i = 0; i < numberOfLocationEntities; i++)
            {
                var nextId = start + i + 1;
                var locationEntity = new LocationEntity
                {
                    Postcode = $"CV{nextId} {nextId + 1}WT",
                    Town = "Coventry",
                    Latitude = 50.1234 + nextId,
                    Longitude = -0.234 - nextId,
                    Website = $"https://test.provider_{nextId}.co.uk",
                    DeliveryYears = new List<DeliveryYearEntity>
                    {
                        new DeliveryYearEntity
                        {
                            Year = (short) (2020 + nextId), 
                            Qualifications = new List<int> {nextId}
                        }
                    },
                    PartitionKey = $"{10000000 + nextId}" //UkPrn
                };

                locationEntity.RowKey = locationEntity.Postcode;
                _locationEntities.Add(locationEntity);
            }

            return this;
        }
    }
}
