using FluentAssertions;
using sfa.Tl.Marketing.Communication.Models.Extensions;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Model.Extensions
{
    public class EntityExtensionsTests
    {
        [Fact]
        public void QualificationList_ToQualificationEntityList_Returns_Expected_Result()
        {
            var qualifications = new QualificationListBuilder().Add(2).Build();

            var qualificationEntities = qualifications.ToQualificationEntityList();

            qualificationEntities.Should().HaveCount(2);
            qualificationEntities[0].Id.Should().Be(qualifications[0].Id);
            qualificationEntities[0].Route.Should().Be(qualifications[0].Route);
            qualificationEntities[0].Name.Should().Be(qualifications[0].Name);

            qualificationEntities[0].Id.Should().Be(1);
            qualificationEntities[0].Route.Should().Be("Route 1");
            qualificationEntities[0].Name.Should().Be("Test Qualification 1");

            qualificationEntities[1].Id.Should().Be(2);
            qualificationEntities[1].Route.Should().Be("Route 2");
            qualificationEntities[1].Name.Should().Be("Test Qualification 2");

        }

        [Fact]
        public void QualificationEntityList_ToQualificationList_Returns_Expected_Result()
        {
            var qualificationEntities = new QualificationEntityListBuilder().Add(2).Build();

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
    }
}
