using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Models;

namespace sfa.Tl.Marketing.Communication.Controllers
{
    public class EmployerController : Controller
    {

        [Route("/employer", Name = "Employer")]
        public IActionResult EmployerHome()
        {
            return View();
        }

        [Route("/employer/skills-available", Name = "EmployerSkills")]
        public IActionResult EmployerSkills()
        {
            return View();
        }

        [Route("/employer/about", Name = "EmployerAbout")]
        public IActionResult EmployerAbout()
        {
            return View();
        }

        [Route("/employer/benefits", Name = "EmployerBenefits")]
        public IActionResult EmployerBenefits()
        {
            return View();
        }

        [Route("/employer/what-it-costs", Name = "EmployerCosts")]
        public IActionResult EmployerCosts()
        {
            return View();
        }
    }
}
