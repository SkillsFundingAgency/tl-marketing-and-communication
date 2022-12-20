using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Entities;
using sfa.Tl.Marketing.Communication.Models.Extensions;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;

namespace sfa.Tl.Marketing.Communication.UnitTests.Model.Extensions;

public class EntityExtensionsTests
{
    [Fact]
    public void QualificationList_ToQualificationEntityList_Returns_Expected_Result()
    {
        var qualifications = new QualificationListBuilder()
            .Add(2)
            .Build();

        var qualificationEntities = qualifications.ToQualificationEntityList();

        qualificationEntities.Should().HaveCount(2);
        qualificationEntities[0].Id.Should().Be(qualifications[0].Id);
        qualificationEntities[0].Route.Should().Be(qualifications[0].Route);
        qualificationEntities[0].Name.Should().Be(qualifications[0].Name);
    }

    [Fact]
    public void QualificationEntityList_ToQualificationList_Returns_Expected_Result()
    {
        var qualificationEntities = new QualificationEntityListBuilder()
            .Add(2)
            .Build();

        var qualifications = qualificationEntities.ToQualificationList();

        qualifications.Should().HaveCount(2);
        qualifications[0].Id.Should().Be(qualificationEntities[0].Id);
        qualifications[0].Route.Should().Be(qualificationEntities[0].Route);
        qualifications[0].Name.Should().Be(qualificationEntities[0].Name);

        qualifications[0].Id.Should().Be(1);
        qualifications[0].Route.Should().Be("Route 1");
        qualifications[0].Name.Should().Be("Test Qualification 1");
        qualifications[1].Id.Should().Be(2);
        qualifications[1].Route.Should().Be("Route 2");
        qualifications[1].Name.Should().Be("Test Qualification 2");
    }

    [Fact]
    public void DeliveryYearEntityList_Serialize_Null_DeliveryYears_Returns_Expected_Result()
    {
        var results = ((IList<DeliveryYearEntity>)null).SerializeDeliveryYears();

        results.Should().Be("[]");
    }

    [Fact]
    public void DeliveryYearEntityList_Serialize_Empty_DeliveryYears_Returns_Expected_Result()
    {
        var deliveryYearEntities = new List<DeliveryYearEntity>();

        var results = deliveryYearEntities.SerializeDeliveryYears();

        results.Should().Be("[]");
    }

    [Fact]
    public void DeliveryYearEntityList_Deserialize_Null_DeliveryYears_Returns_Expected_Result()
    {
        var results = ((string)null).DeserializeDeliveryYears();

        results.Should().NotBeNull();
        results.Should().BeEmpty();
    }

    [Fact]
    public void DeliveryYearEntityList_Deserialize_Empty_DeliveryYears_Returns_Expected_Result()
    {
        var deliveryYearString = string.Empty;

        var results = deliveryYearString.DeserializeDeliveryYears();

        results.Should().NotBeNull();
        results.Should().BeEmpty();
    }

    [Fact]
    public void DeliveryYearEntityList_Serialize_And_Deserialize_DeliveryYears_Round_Trip_Returns_Expected_Result()
    {
        var deliveryYearEntities = new DeliveryYearEntityListBuilder()
            .Add(2)
            .Build();

        var deliveryYearString = deliveryYearEntities.SerializeDeliveryYears();

        var results = deliveryYearString.DeserializeDeliveryYears();

        results.Should().BeEquivalentTo(deliveryYearEntities);
    }

    [Fact]
    public void TownList_ToTownEntityList_Returns_Expected_Result()
    {
        var towns = new TownListBuilder()
            .Add(2)
            .Build();

        var townEntities = towns.ToTownEntityList();

        townEntities.Should().HaveCount(2);
        townEntities[0].Id.Should().Be(towns[0].Id);
        townEntities[0].Name.Should().Be(towns[0].Name);
        townEntities[0].County.Should().Be(towns[0].County);
        townEntities[0].LocalAuthority.Should().Be(towns[0].LocalAuthority);
        townEntities[0].Latitude.Should().Be(towns[0].Latitude);
        townEntities[0].Longitude.Should().Be(towns[0].Longitude);
        townEntities[0].SearchString.Should().Be(towns[0].SearchString);

        townEntities[0].PartitionKey.Should().Be(towns[0].Name[..3].ToUpper());
        townEntities[0].RowKey.Should().Be(towns[0].Id.ToString());

        townEntities[1].Id.Should().Be(towns[1].Id);
        townEntities[1].Name.Should().Be(towns[1].Name);
        townEntities[1].County.Should().Be(towns[1].County);
        townEntities[1].LocalAuthority.Should().Be(towns[1].LocalAuthority);
        townEntities[1].Latitude.Should().Be(towns[1].Latitude);
        townEntities[1].Longitude.Should().Be(towns[1].Longitude);
        townEntities[1].SearchString.Should().Be(towns[1].SearchString);

        townEntities[1].PartitionKey.Should().Be(towns[1].Name[..3].ToUpper());
        townEntities[1].RowKey.Should().Be(towns[1].Id.ToString());
    }

    [Fact]
    public void TownEntityList_ToTownList_Returns_Expected_Result()
    {
        var townEntities = new TownEntityListBuilder()
            .Add(2)
            .Build();

        var towns = townEntities.ToTownList();

        towns.Should().HaveCount(2);
        towns[0].Id.Should().Be(townEntities[0].Id);
        towns[0].Name.Should().Be(townEntities[0].Name);
        towns[0].County.Should().Be(townEntities[0].County);
        towns[0].LocalAuthority.Should().Be(townEntities[0].LocalAuthority);
        towns[0].Latitude.Should().Be(townEntities[0].Latitude);
        towns[0].Longitude.Should().Be(townEntities[0].Longitude);
        towns[0].SearchString.Should().Be(townEntities[0].SearchString);

        towns[1].Id.Should().Be(townEntities[1].Id);
        towns[1].Name.Should().Be(townEntities[1].Name);
        towns[1].County.Should().Be(townEntities[1].County);
        towns[1].LocalAuthority.Should().Be(townEntities[1].LocalAuthority);
        towns[1].Latitude.Should().Be(townEntities[1].Latitude);
        towns[1].Longitude.Should().Be(townEntities[1].Longitude);
        towns[1].SearchString.Should().Be(townEntities[1].SearchString);
    }
}