using System;
using System.Globalization;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Constants;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Models.Extensions;

namespace sfa.Tl.Marketing.Communication.SearchPipeline.Steps;

public class ValidateSearchTermAndLoadLocationStep : ISearchStep
{
    private readonly IProviderSearchService _providerSearchService;
    private readonly ITownDataService _townDataService;

    public ValidateSearchTermAndLoadLocationStep(
        IProviderSearchService providerSearchService,
        ITownDataService townDataService)
    {
        _providerSearchService = providerSearchService ?? throw new ArgumentNullException(nameof(providerSearchService));
        _townDataService = townDataService ?? throw new ArgumentNullException(nameof(townDataService));
    }

    public async Task Execute(ISearchContext context)
    {
        if (string.IsNullOrEmpty(context.ViewModel.SearchTerm))
        {
            context.ViewModel.ValidationMessage = AppConstants.PostcodeValidationMessage;
        }
        else
        {
            if (context.ViewModel.SearchTerm.Replace(" ", "").IsFullOrPartialPostcode())
            {
                var (isValid, postcodeLocation) =
                    await _providerSearchService
                        .IsSearchPostcodeValid(context.ViewModel.SearchTerm);
                SetViewModelDetails(context, 
                    isValid, 
                    postcodeLocation?.Postcode, 
                    postcodeLocation?.Latitude, 
                    postcodeLocation?.Longitude);
            }
            else
            {
                var (isValid, town) = 
                    await _townDataService
                        .IsSearchTermValid(context.ViewModel.SearchTerm);
                SetViewModelDetails(context, 
                    isValid, 
                    town?.DisplayName, 
                    town?.Latitude, 
                    town?.Longitude);
            }
        }

        if (!string.IsNullOrEmpty(context.ViewModel.ValidationMessage))
        {
            context.ViewModel.ValidationStyle = AppConstants.ValidationStyle;
            context.Continue = false;
        }
    }

    private static void SetViewModelDetails(
        ISearchContext context,
        bool isValid,
        string displayName,
        double? latitude,
        double? longitude)
    {
        if (isValid)
        {
            context.ViewModel.SearchTerm = displayName;
            context.ViewModel.Latitude = latitude.HasValue
                ? latitude.Value.ToString(CultureInfo.InvariantCulture)
                : "";
            context.ViewModel.Longitude = longitude.HasValue
                ? longitude.Value.ToString(CultureInfo.InvariantCulture)
                : "";
        }
        else
        {
            context.ViewModel.ValidationMessage = AppConstants.RealPostcodeOrTownValidationMessage;
        }
    }
}