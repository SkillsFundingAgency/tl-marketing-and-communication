using Microsoft.AspNetCore.Mvc;

namespace sfa.Tl.Marketing.Communication.Controllers
{
    public class EmployerController : Controller
    {
        [Route("/employers", Name = "Employer")]
        public IActionResult EmployerHome()
        {
            return View();
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

        [Route("/employers/video-transcript", Name = "EmployerVideoTranscript")]
        public IActionResult EmployerVideoTranscript()
        {
            return View();
        }

        
    }
}
