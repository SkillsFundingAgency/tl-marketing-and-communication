using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using sfa.Tl.Marketing.Communication.Constants;

namespace sfa.Tl.Marketing.Communication.Models;

public class FindViewModel
{
    public string SearchTerm { get; set; }
    public string Longitude { get; set; }
    public string Latitude { get; set; }
    public string Qualification { get; init; }
    public bool ShouldSearch { get; set; }
    public int? NumberOfItemsToShow { get; set; }
    public IEnumerable<ProviderLocationViewModel> ProviderLocations { get; set; } = new List<ProviderLocationViewModel>();
    public IEnumerable<SelectListItem> Qualifications { get; set; } = new List<SelectListItem>();
    public int? SelectedQualificationId { get; set; }
    public int SearchedQualificationId { get; set; }
    public bool ShowNext 
    {
        get
        {
            var showNext = ProviderLocations.Count() >= AppConstants.DefaultNumberOfItemsToShow
                           && TotalRecordCount.HasValue && NumberOfItemsToShow.HasValue
                           && TotalRecordCount.Value > NumberOfItemsToShow.Value;
            return showNext;
        } 
    }

    public string ValidationMessage { get; set; }
    public string ValidationStyle { get; set; }
    public int? TotalRecordCount { get; set; }
    public int SelectedItemIndex { get; set; }
    public SearchSubmitType SubmitType { get; set; }
}