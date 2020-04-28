using Microsoft.AspNetCore.Mvc.Rendering;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
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
                context.ViewModel.PostCodeValidationMessage = "You must enter a postcode";
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
                    context.ViewModel.PostCodeValidationMessage = "You must enter a real postcode";
                }
            }

           if (!string.IsNullOrEmpty(context.ViewModel.PostCodeValidationMessage))
            {
                context.ViewModel.ValidationStyle = "tl-validation--error";
                context.Continue = false;
            }
           
        }
    }
}
