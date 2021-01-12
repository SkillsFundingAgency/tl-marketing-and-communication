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
                    context.ViewModel.Postcode = results.PostcodeLocation.Postcode;
                    context.ViewModel.Latitude = results.PostcodeLocation.Latitude.Value.ToString(CultureInfo.InvariantCulture);
                    context.ViewModel.Longitude = results.PostcodeLocation.Longitude.Value.ToString(CultureInfo.InvariantCulture);
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
