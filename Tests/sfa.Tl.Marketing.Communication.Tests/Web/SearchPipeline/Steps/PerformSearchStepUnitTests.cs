using AutoMapper;
using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using sfa.Tl.Marketing.Communication.SearchPipeline.Steps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.SearchPipeline.Steps
{
    public class PerformSearchStepUnitTests
    {
        private readonly IProviderSearchService _providerSearchService;
        private readonly ISearchStep _searchStep;
        private readonly IMapper _mapper;

        public PerformSearchStepUnitTests()
        {
            _providerSearchService = Substitute.For<IProviderSearchService>();
            _mapper = Substitute.For<IMapper>();
            _searchStep = new PerformSearchStep(_providerSearchService, _mapper);
        }

        [Fact]
        public async Task Step_Perform_Search()
        {
            // Arrange         
            var postcode = "MK35 8UK";
            var numberOfItems = 9;
            var qualificationId = 5;
            var selectedItemIndex = 3;

            var viewModel = new FindViewModel
            {
                Postcode = postcode,
                NumberOfItemsToShow = numberOfItems,
                SelectedQualificationId = qualificationId,
                SelectedItemIndex = selectedItemIndex
            };

            var context = new SearchContext(viewModel);
            
            var providerLocations = new List<ProviderLocation>
            {
                new ProviderLocation(),
                new ProviderLocation(),
                new ProviderLocation(),
                new ProviderLocation(),
                new ProviderLocation()
            };

            _providerSearchService.Search(Arg.Is<SearchRequest>(sr => sr.Postcode == postcode && 
            sr.NumberOfItems == numberOfItems && 
            sr.QualificationId == qualificationId))
                .Returns((providerLocations.Count(), providerLocations));
            
            var providerLocationViewModels = new List<ProviderLocationViewModel>
            {
                new ProviderLocationViewModel(),
                new ProviderLocationViewModel(),
                new ProviderLocationViewModel(),
                new ProviderLocationViewModel(),
                new ProviderLocationViewModel()
            };

            _mapper.Map<IEnumerable<ProviderLocationViewModel>>(Arg.Is(providerLocations)).Returns(providerLocationViewModels);

            // Act
            await _searchStep.Execute(context);

            // Assert
            context.ViewModel.TotalRecordCount.Should().Be(providerLocationViewModels.Count);
            providerLocationViewModels[context.ViewModel.SelectedItemIndex].HasFocus.Should().BeTrue();
            context.ViewModel.SearchedQualificationId.Should().Be(qualificationId);
            context.ViewModel.ProviderLocations.Should().Equal(providerLocationViewModels);
            await _providerSearchService.Received(1).Search(Arg.Is<SearchRequest>(sr => sr.Postcode == postcode && sr.NumberOfItems == numberOfItems && sr.QualificationId == qualificationId));
            _mapper.Received(1).Map<IEnumerable<ProviderLocationViewModel>>(Arg.Is(providerLocations));

        }
    }
}
