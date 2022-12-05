using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Entities;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

public class TownEntityListBuilder
{
    private readonly IList<TownEntity> _townEntities = new List<TownEntity>();

    public IList<TownEntity> Build() =>
        _townEntities;

    public TownEntityListBuilder Add(int numberOfTownEntities = 1)
    {
        var start = _townEntities.Count;
        for (var i = 0; i < numberOfTownEntities; i++)
        {
            var nextId = start + i + 1;
            var town = new TownEntity
            {
                Id = nextId,
                Name = $"Test Town {nextId}",
                County = $"County {nextId}",
                LocalAuthority = $"Local Authority {nextId}",
                Latitude = double.Parse($"50.0{nextId}"),
                Longitude = double.Parse($"-1.0{nextId}"),
                // ReSharper disable once StringLiteralTypo
                SearchString = $"testtown{nextId}county{nextId}",
                PartitionKey = "T"
            };
            town.RowKey = town.Id.ToString();
            _townEntities.Add(town);
        }

        return this;
    }
}