using FluentAssertions;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Model
{
    public class FindViewModelTests
    {
        [Fact]
        public void FindViewModel_Default_Collections_Are_Not_Null()
        {
            var viewModel = new FindViewModel();

            viewModel.ProviderLocations.Should().NotBeNull();
            viewModel.Qualifications.Should().NotBeNull();
        }

        [Fact]
        public void FindViewModel_ShowNext_Should_Be_True_When_NumberOfItemsToShow_Is_Less_Than_TotalRecordCount()
        {
            var viewModel = new FindViewModel
            {
                ProviderLocations = new ProviderLocationViewModelListBuilder()
                    .Add(5)
                    .Build(),
                NumberOfItemsToShow = 15,
                TotalRecordCount = 20
            };

            viewModel.ShowNext.Should().BeTrue();
        }

        [Fact]
        public void FindViewModel_ShowNext_Should_Be_False_When_TotalRecordCount_Is_Equal_To_ProviderLocations_Count()
        {
            var viewModel = new FindViewModel
            {
                ProviderLocations = new ProviderLocationViewModelListBuilder()
                    .Add(5)
                    .Build(),
                NumberOfItemsToShow = 5,
                TotalRecordCount = 5
            };

            viewModel.ShowNext.Should().BeFalse();
        }
    }
}
