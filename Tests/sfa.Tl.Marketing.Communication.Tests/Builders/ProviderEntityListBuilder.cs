using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Entities.AzureDataTables;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

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
            var provider = new ProviderEntity
            {
                UkPrn = 10000000 + nextId, 
                Name = $"Test Provider {nextId}", 
                PartitionKey = "providers"
            };
            provider.RowKey = provider.UkPrn.ToString();
            _providerEntities.Add(provider);
        }

        return this;
    }
}