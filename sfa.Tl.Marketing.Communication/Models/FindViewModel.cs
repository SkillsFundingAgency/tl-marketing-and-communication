using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace sfa.Tl.Marketing.Communication.Models
{
    public class FindViewModel
    {
        [Required]
        public string Postcode { get; set; }
        public string Qualification { get; set; }
        public bool ShouldSearch { get; set; }
        public int? NumberOfItems { get; set; }
        public string MapApiKey { get; set; }
        public IEnumerable<ProviderLocationViewModel> ProviderLocations { get; set; } = new List<ProviderLocationViewModel>();
        public IEnumerable<SelectListItem> Qualifications { get; set; } = new List<SelectListItem>();
        public int? SelectedQualificationId { get; set; }
        public bool ShowNext 
        {
            get
            {
                return ProviderLocations.Count() >= 5;
            } 
        }
    }
} 