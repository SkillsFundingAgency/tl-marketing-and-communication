using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Entities;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

public class DeliveryYearEntityListBuilder
{
    private readonly IList<DeliveryYearEntity> _deliveryYearEntities = new List<DeliveryYearEntity>();

    public IList<DeliveryYearEntity> Build() =>
        _deliveryYearEntities;

    public DeliveryYearEntityListBuilder Add(int numberOfDeliveryYearEntities = 1)
    {
        var start = _deliveryYearEntities.Count;
        for (var i = 0; i < numberOfDeliveryYearEntities; i++)
        {
            var deliveryYear = new DeliveryYearEntity
            {
                Year = (short)(2020 + i),
                Qualifications = new List<int>()
                {
                    1,
                    2
                }
            };
            _deliveryYearEntities.Add(deliveryYear);
        }

        return this;
    }
}