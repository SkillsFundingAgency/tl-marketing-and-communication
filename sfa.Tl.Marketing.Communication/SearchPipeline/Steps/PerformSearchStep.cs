using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.SearchPipeline.Steps
{
    public class PerformSearchStep : ISearchStep
    {
        private readonly IProviderSearchService _providerSearchService;
        private readonly IMapper _mapper;

        public PerformSearchStep(IProviderSearchService providerSearchService, IMapper mapper)
        {
            _providerSearchService = providerSearchService;
            _mapper = mapper;
        }

        public async Task Execute(ISearchContext context)
        {
            var searchRequest = new SearchRequest { Postcode = context.ViewModel.Postcode, 
                NumberOfItems = context.ViewModel.NumberOfItemsToShow.Value, 
                QualificationId = context.ViewModel.SelectedQualificationId };
            
            var results = await _providerSearchService.Search(searchRequest);
            
            var providerViewModels = _mapper.Map<IEnumerable<ProviderLocationViewModel>>(results.searchResults).ToList();
            
            context.ViewModel.TotalRecordCount = results.totalCount;
            providerViewModels[context.ViewModel.SelectedItemIndex].HasFocus = true;

            context.ViewModel.ProviderLocations = providerViewModels;
            context.ViewModel.SearchedQualificationId = context.ViewModel.SelectedQualificationId.Value;
        }
    }
}
