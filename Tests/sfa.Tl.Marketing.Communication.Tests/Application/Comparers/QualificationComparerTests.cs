using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.Comparers;
using sfa.Tl.Marketing.Communication.Models.Dto;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Comparers
{
    public class QualificationComparerTests
    {
        [Fact]
        public void QualificationComparerTests_Returns_True_For_Same_Values()
        {
            var q1 = new Qualification
            {
                Id = 1,
                Name = "Test 1"
            };

            var q2 = new Qualification
            {
                Id = 1,
                Name = "Test 1"
            };

            var comparer = new QualificationComparer();

            comparer.Equals(q1, q2).Should().BeTrue();
        }

        [Fact]
        public void QualificationComparerTests_Returns_True_For_Same_Object()
        {
            var q1 = new Qualification
            {
                Id = 1,
                Name = "Test 1"
            };

            var comparer = new QualificationComparer();

            comparer.Equals(q1, q1).Should().BeTrue();
        }

        [Fact]
        public void QualificationComparerTests_Returns_False_For_Different_Names()
        {
            var q1 = new Qualification
            {
                Id = 1,
                Name = "Test 1"
            };

            var q2 = new Qualification
            {
                Id = 1,
                Name = "Test 2"
            };

            var comparer = new QualificationComparer();

            comparer.Equals(q1, q2).Should().BeTrue();
        }
        [Fact]
        public void QualificationComparerTests_Returns_Not_Equals_For_Q1_Null()
        {
            var q2 = new Qualification
            {
                Id = 1,
                Name = "Test 1"
            };

            var comparer = new QualificationComparer();

            comparer.Equals(null, q2).Should().BeFalse();
        }

        [Fact]
        public void QualificationComparerTests_Returns_Not_Equals_For_Q2_Null()
        {
            var q1 = new Qualification
            {
                Id = 1,
                Name = "Test 1"
            };
            
            var comparer = new QualificationComparer();

            comparer.Equals(q1, null).Should().BeFalse();
        }

        [Fact]
        public void QualificationComparerTests_Returns_Not_Equal_For_Both_Null()
        {
            var comparer = new QualificationComparer();

            comparer.Equals(null, null).Should().BeFalse();
        }
    }
}
