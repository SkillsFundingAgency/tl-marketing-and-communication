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
                    //TODO: Add the rest of the entity fields
                });
            }

            return this;
        }
    }
}
