using Microsoft.AspNetCore.Mvc.Rendering;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.SearchPipeline.Steps
{
    public class ValidatePostcodeStep : ISearchStep
    {
        private readonly IProviderSearchService _providerSearchService;

        public ValidatePostcodeStep(IProviderSearchService providerSearchService)
        {
            _providerSearchService = providerSearchService;
        }

        public async Task Execute(ISearchContext context)
        {
            if (string.IsNullOrEmpty(context.ViewModel.Postcode))
            {
                context.ViewModel.PostcodeValidationMessage = AppConstants.PostcodeValidationMessage;
            }
            else
            {
                var results = await _providerSearchService.IsSearchPostcodeValid(context.ViewModel.Postcode);
                
                if (results.IsValid)
                {
                    context.ViewModel.Postcode = results.Postcode;
                }
                else
                {
                    context.ViewModel.PostcodeValidationMessage = AppConstants.RealPostcodeValidationMessage;
                }
            }

           if (!string.IsNullOrEmpty(context.ViewModel.PostcodeValidationMessage))
            {
                context.ViewModel.ValidationStyle = AppConstants.ValidationStyle;
                context.Continue = false;
            }
           
        }
    }
}
