using System.ComponentModel.DataAnnotations;

namespace sfa.Tl.Marketing.Communication.Models
{
    public class EmployerContactViewModel
    {
        [Required(ErrorMessage = "You must enter your full name")]
        [MinLength(2, ErrorMessage = "You must enter a contact name using 2 or more characters")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "You must enter your organisation’s name")]
        public string OrganisationName { get; set; }

        [Required(ErrorMessage = "You must enter your telephone number")]
        public string Phone { get; set; }
        
        [Required(ErrorMessage = "You must enter your email address")]
        [RegularExpression(@"^[a-zA-Z0-9\u0080-\uFFA7?$#()""'!,+\-=_:;.&€£*%\s\/]+@[a-zA-Z0-9\u0080-\uFFA7?$#()""'!,+\-=_:;.&€£*%\s\/]+\.([a-zA-Z0-9\u0080-\uFFA7]{2,10})$", 
            ErrorMessage = "You must enter a valid email")]
        public string Email { get; set; }
       

        public bool ContactFormSent { get; set; }
    }
}