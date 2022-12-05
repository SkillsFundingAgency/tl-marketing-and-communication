using System;
using System.Globalization;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Constants;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Models.Extensions;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.SearchPipeline.Steps;

public class ValidatePostcodeStep : ISearchStep
{
    private readonly ITownDataService _townDataService;
    private readonly IProviderSearchService _providerSearchService;

    public ValidatePostcodeStep(
        IProviderSearchService providerSearchService,
        ITownDataService townDataService)
    {
        _providerSearchService = providerSearchService ?? throw new ArgumentNullException(nameof(providerSearchService));
        _townDataService = townDataService ?? throw new ArgumentNullException(nameof(townDataService));
    }

    public async Task Execute(ISearchContext context)
    {
        if (string.IsNullOrEmpty(context.ViewModel.Postcode))
        {
            context.ViewModel.ValidationMessage = AppConstants.PostcodeValidationMessage;
        }
        else
        {
            if (context.ViewModel.Postcode.Replace(" ", "").IsFullOrPartialPostcode())
            {
                var (isValid, postcodeLocation) =
                    await _providerSearchService.IsSearchPostcodeValid(context.ViewModel.Postcode);

                if (isValid)
                {
                    context.ViewModel.Postcode = postcodeLocation.Postcode;
                    context.ViewModel.Latitude = postcodeLocation.Latitude.HasValue
                        ? postcodeLocation.Latitude.Value.ToString(CultureInfo.InvariantCulture)
                        : "";
                    context.ViewModel.Longitude = postcodeLocation.Longitude.HasValue
                        ? postcodeLocation.Longitude.Value.ToString(CultureInfo.InvariantCulture)
                        : "";
                }
                else
                {
                    context.ViewModel.ValidationMessage = AppConstants.RealPostcodeValidationMessage;
                }
            }
            else
            {
                var (isValid, town) =
                    await _townDataService.IsSearchTermValid(context.ViewModel.Postcode);

                if (isValid)
                {
                    context.ViewModel.Postcode = town.DisplayName;
                    context.ViewModel.Latitude = town.Latitude.ToString(CultureInfo.InvariantCulture);
                    context.ViewModel.Longitude = town.Longitude.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    context.ViewModel.ValidationMessage = AppConstants.RealPostcodeOrTownValidationMessage;
                }
            }
        }

        if (!string.IsNullOrEmpty(context.ViewModel.ValidationMessage))
        {
            context.ViewModel.ValidationStyle = AppConstants.ValidationStyle;
            context.Continue = false;
        }
    }
}