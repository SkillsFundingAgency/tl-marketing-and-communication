using System.ComponentModel.DataAnnotations;
using sfa.Tl.Marketing.Communication.Application.Enums;

namespace sfa.Tl.Marketing.Communication.Models
{
    public class EmployerContactViewModel
    {
        [Required(ErrorMessage = "You must enter a full name")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "You must enter an organisation name")]
        public string OrganisationName { get; set; }

        [Required(ErrorMessage = "You must enter a telephone number")]
        public string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "You must enter a valid email address")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "You must select a contact method")]
        public ContactMethod? ContactMethod { get; set; }
    }
}