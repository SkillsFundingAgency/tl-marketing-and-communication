using System;
using System.Collections.Generic;
using System.Linq;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Extensions;

public class BusinessRuleExtensionsTests
{
    [Theory(DisplayName = nameof(BusinessRuleExtensions.IsAvailableAtDate) + " Data Tests")]
    [InlineData(2020, "2020-12-31", true)]
    [InlineData(2021, "2020-12-31", false)]
    [InlineData(2021, "2021-08-31", false)]
    [InlineData(2021, "2021-09-01", true)]
    [InlineData(2021, "2022-09-01", true)]
    [InlineData(2022, "2022-08-31", false)]
    [InlineData(2022, "2022-09-01", true)]
    [InlineData(2023, "2023-08-31", false)]
    [InlineData(2023, "2023-09-01", true)]
    public void DeliveryYear_IsAvailableAtDate_Data_Tests(short deliveryYear, string currentDate,
        bool expectedResult)
    {
        var today = DateTime.Parse(currentDate);

        var result = deliveryYear.IsAvailableAtDate(today);
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void GetQualificationsForDeliveryYear_Returns_Expected_Result_For_Null_DeliveryYearDto()
    {
        var qualificationsDictionary
            = new Dictionary<int, Qualification>();

        var result = 
            ((DeliveryYearDto)null, qualificationsDictionary)
            .GetQualificationsForDeliveryYear();

        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [Fact]
    public void GetQualificationsForDeliveryYear_Returns_Expected_Result_For_EmptyDeliveryYearDto()
    {
        var deliveryYear = new DeliveryYearDto();

        var qualificationsDictionary
            = new Dictionary<int, Qualification>();

        var result = 
            (deliveryYear, qualificationsDictionary)
            .GetQualificationsForDeliveryYear();

        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [Fact]
    public void GetQualificationsForDeliveryYear_Returns_Expected_Result()
    {
        var deliveryYear = new DeliveryYearDtoBuilder().Build();
        
        var qualifications = new QualificationListBuilder()
            .Add(3)
            .Build();

        var qualificationsDictionary = qualifications
            .ToDictionary(q => q.Id);

        var result = 
            (deliveryYear, qualificationsDictionary)
            .GetQualificationsForDeliveryYear();

        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Test Qualification 1");
    }

    [Fact]
    public void GetQualificationsForDeliveryYear_Returns_Expected_Result_With_Missing_Qualification()
    {
        var deliveryYear = new DeliveryYearDtoBuilder().Build();

        var qualifications = new QualificationListBuilder()
            .Add(3)
            .Remove(1)
            .Build();

        var qualificationsDictionary = qualifications
            .ToDictionary(q => q.Id);

        var result =
            (deliveryYear, qualificationsDictionary)
            .GetQualificationsForDeliveryYear();

        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }
}