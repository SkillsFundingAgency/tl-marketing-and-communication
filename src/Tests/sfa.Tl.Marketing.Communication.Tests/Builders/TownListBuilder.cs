using System.Collections.Generic;
using System.Linq;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

internal class TownListBuilder
{
    private readonly IList<Town> _towns = new List<Town>();

    public IList<Town> Build() =>
        _towns;

    public TownListBuilder Add(int numberOfTowns = 1)
    {
        if (_towns != null)
        {
            var start = _towns.Count;
            for (var i = 0; i < numberOfTowns; i++)
            {
                var nextId = start + i + 1;
                _towns.Add(new Town
                {
                    Id = nextId,
                    Name = $"Test Town {nextId}",
                    County = $"County {nextId}",
                    LocalAuthority = $"Local Authority {nextId}",
                    Latitude = double.Parse($"50.0{nextId}"),
                    Longitude = double.Parse($"-1.0{nextId}"),
                    // ReSharper disable once StringLiteralTypo
                    SearchString = $"testtown{nextId}county{nextId}"
                });
            }
        }

        return this;
    }

    public TownListBuilder Remove(int id)
    {
        var itemToRemove = _towns
            .SingleOrDefault(q => q.Id == id);

        if (itemToRemove is not null)
        {
            _towns.Remove(itemToRemove);
        }

        return this;
    }

    public TownListBuilder CreateKnownList()
    {
        _towns.Clear();
        _towns.Add(new()
        {
            Id = 1,
            Name = "Coventry",
            County = "West Midlands",
            LocalAuthority = "West Midlands",
            Latitude = 52.41695,
            Longitude = -1.50721,
            // ReSharper disable once StringLiteralTypo
            SearchString = "coventrywestmidlands"
        });
        _towns.Add(new()
        {
            Id = 2,
            Name = "Oxford",
            County = "Oxfordshire",
            LocalAuthority = "Oxfordshire",
            Latitude = 51.740811,
            Longitude = -1.217524,
            // ReSharper disable once StringLiteralTypo
            SearchString = "oxfordoxfordshire"
        });

        return this;
    }
}