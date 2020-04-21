using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace sfa.Tl.Marketing.Communication.Models
{
    public class FindViewModel
    {
        [Required(ErrorMessage = "You must enter a postcode")]
        public string Postcode { get; set; }
        public string Qualification { get; set; }
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
                return ProviderLocations.Count() >= 5;
            } 
        }

        public string PostCodeValidationMessage { get; set; }
        public string ValidationStyle { get; set; }
        public bool ResetNumberOfItems { get; set; }
        public int? TotalRecordCount { get; set; }
        public int SelectedItemIndex { get; set; }
    }
} 