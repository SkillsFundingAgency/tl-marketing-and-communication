using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
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
            return View();
        }

        [HttpPost]
        [Route("/employers/next-steps", Name = "EmployerNextSteps")]
        public async Task<IActionResult> EmployerNextSteps(EmployerContactViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var emailSent = await _emailService.SendEmployerEmail(viewModel.FullName, viewModel.OrganisationName,
                viewModel.PhoneNumber, viewModel.Email, viewModel.ContactMethod.Value);

            return View(viewModel);
        }

        [Route("/employers/video-transcript", Name = "EmployerVideoTranscript")]
        public IActionResult EmployerVideoTranscript()
        {
            return View();
        }
    }
}
