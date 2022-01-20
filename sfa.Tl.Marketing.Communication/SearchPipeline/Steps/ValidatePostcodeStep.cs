using System;
using System.Globalization;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Constants;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.SearchPipeline.Steps
{
    public class ValidatePostcodeStep : ISearchStep
    {
        private readonly IProviderSearchService _providerSearchService;
        
        public ValidatePostcodeStep(IProviderSearchService providerSearchService)
        {
            _providerSearchService = providerSearchService ?? throw new ArgumentNullException(nameof(providerSearchService));
        }

        public async Task Execute(ISearchContext context)
        {
            if (string.IsNullOrEmpty(context.ViewModel.Postcode))
            {
                context.ViewModel.PostcodeValidationMessage = AppConstants.PostcodeValidationMessage;
            }
            else
            {
                var (isValid, postcodeLocation) = await _providerSearchService.IsSearchPostcodeValid(context.ViewModel.Postcode);

                if (isValid)
                {
                    context.ViewModel.Postcode = postcodeLocation.Postcode;
                    context.ViewModel.Latitude = postcodeLocation.Latitude.HasValue ? postcodeLocation.Latitude.Value.ToString(CultureInfo.InvariantCulture) : "";
                    context.ViewModel.Longitude = postcodeLocation.Longitude.HasValue ? postcodeLocation.Longitude.Value.ToString(CultureInfo.InvariantCulture) : "";
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
