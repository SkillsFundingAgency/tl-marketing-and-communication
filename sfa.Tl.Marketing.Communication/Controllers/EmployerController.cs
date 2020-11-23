using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Application.Enums;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Constants;
using sfa.Tl.Marketing.Communication.Models;

namespace sfa.Tl.Marketing.Communication.Controllers
{
    public class EmployerController : Controller
    {
        private readonly IEmailService _emailService;

        public EmployerController(IEmailService emailService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        [Route("/employers", Name = "Employer")]
        public IActionResult EmployerHome()
        {
            return View();
        }

        [Route("/employer")]
        public IActionResult EmployerHomeRedirect()
        {
            return RedirectToAction(nameof(EmployerHome));
        }

        [Route("/employers/skills-available", Name = "EmployerSkills")]
        public IActionResult EmployerSkills()
        {
            return View();
        }

        [Route("/employers/about", Name = "EmployerAbout")]
        public IActionResult EmployerAbout()
        {
            return View();
        }

        [Route("/employers/business-benefits", Name = "EmployerBenefits")]
        public IActionResult EmployerBenefits()
        {
            return View();
        }

        [Route("/employers/what-it-costs", Name = "EmployerCosts")]
        public IActionResult EmployerCosts()
        {
            return View();
        }

        [Route("/employers/next-steps", Name = "EmployerNextSteps")]
        public IActionResult EmployerNextSteps()
        {
            return View(new EmployerContactViewModel
            {
                ContactFormSent = HasContactFormSentCookie()
            });
        }

        [HttpPost]
        [Route("/employers/next-steps", Name = "EmployerNextSteps")]
        public async Task<IActionResult> EmployerNextSteps(EmployerContactViewModel viewModel)
        {
            Validate(viewModel);

            if (!ModelState.IsValid || viewModel.ContactFormSent || HasContactFormSentCookie())
            {
                return View(viewModel);
            }
            
            if (HasContactFormSentCookie())
            {

            }

            if (!(await _emailService.SendEmployerContactEmail(
                viewModel.FullName, 
                viewModel.OrganisationName,
                viewModel.Phone, 
                viewModel.Email, 
                viewModel.ContactMethod ?? ContactMethod.Email)))
            {
                return View("Error", new ErrorViewModel());
            }

            viewModel.ContactFormSent = true;

            Response.Cookies.Append(AppConstants.EmployerContactFormSentCookieName, "true",
                new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(365),
                    IsEssential = true,
                    HttpOnly = false,
                    Secure = true
                });

            return View(viewModel);
        }

        [Route("/employers/video-transcript", Name = "EmployerVideoTranscript")]
        public IActionResult EmployerVideoTranscript()
        {
            return View();
        }

        private bool HasContactFormSentCookie()
        {
            return Request.Cookies.ContainsKey(AppConstants.EmployerContactFormSentCookieName)
                   && Request.Cookies[AppConstants.EmployerContactFormSentCookieName] == "true";
        }

        private void Validate(EmployerContactViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.Phone))
                return;

            if (!viewModel.Phone.Any(char.IsDigit))
                ModelState.AddModelError(nameof(viewModel.Phone), "You must enter a number");
            else if (!Regex.IsMatch(viewModel.Phone, @"^(?:.*\d.*){7,}$"))
                ModelState.AddModelError(nameof(viewModel.Phone), "You must enter a telephone number that has 7 or more numbers");
        }
    }
}
