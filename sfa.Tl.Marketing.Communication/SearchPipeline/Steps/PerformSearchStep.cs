﻿using System;
using AutoMapper;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.SearchPipeline.Steps
{
    public class PerformSearchStep : ISearchStep
    {
        private readonly IDateTimeService _dateTimeService;
        private readonly IProviderSearchService _providerSearchService;
        private readonly IMapper _mapper;

        public PerformSearchStep(IProviderSearchService providerSearchService,
            IDateTimeService dateTimeService,
            IMapper mapper)
        {
            _providerSearchService = providerSearchService ?? throw new ArgumentNullException(nameof(providerSearchService));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Execute(ISearchContext context)
        {
            var searchRequest = new SearchRequest
            {
                Postcode = context.ViewModel.Postcode,
                OriginLatitude = context.ViewModel.Latitude,
                OriginLongitude = context.ViewModel.Longitude,
                NumberOfItems = context.ViewModel.NumberOfItemsToShow ?? 0,
                QualificationId = context.ViewModel.SelectedQualificationId
            };

            var (totalCount, searchResults) = await _providerSearchService.Search(searchRequest);

            var providerViewModels = _mapper.Map<IEnumerable<ProviderLocationViewModel>>(searchResults).ToList();

            context.ViewModel.TotalRecordCount = totalCount;
            if (providerViewModels.Count > context.ViewModel.SelectedItemIndex)
            {
                providerViewModels[context.ViewModel.SelectedItemIndex].HasFocus = true;
            }

            context.ViewModel.ProviderLocations = providerViewModels;
            context.ViewModel.SearchedQualificationId = context.ViewModel.SelectedQualificationId ?? -1;
        }
    }
}
