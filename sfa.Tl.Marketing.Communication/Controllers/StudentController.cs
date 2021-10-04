using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;

namespace sfa.Tl.Marketing.Communication.Controllers
{
    public class StudentController : Controller
    {
        private readonly IProviderDataService _providerDataService;
        private readonly IProviderSearchEngine _providerSearchEngine;

        public StudentController(
            IProviderDataService providerDataService,
            IProviderSearchEngine providerSearchEngine)
        {
            _providerSearchEngine = providerSearchEngine ?? throw new ArgumentNullException(nameof(providerSearchEngine));
            _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        }

        [Route("/students", Name = "Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/students/about", Name = "About")]
        public IActionResult About()
        {
            return View();
        }

        [Route("/students/why", Name = "Why")]
        public IActionResult Why()
        {
            return View();
        }

        [Route("/students/subjects", Name = "Subjects")]
        public IActionResult Subjects()
        {
            return View();
        }


        [Route("/students/case-studies", Name = "CaseStudies")]
        public IActionResult CaseStudies()
        {
            return View();
        }

        [Route("/students/video-transcript", Name = "StudentVideoTranscript")]
        public IActionResult StudentVideoTranscript()
        {
            return View();
        }

        [Route("/students/find", Name = "Find")]
        public async Task<IActionResult> Find(FindViewModel viewModel)
        {
            var searchResults = await _providerSearchEngine.Search(viewModel);

            return View(searchResults);
        }

        [Route("/students/subjects/accounting", Name = "Accounting")]
        public IActionResult Accounting()
        {
            return View("Subjects/Accounting");
        }

        [Route("/students/subjects/building-services-engineering", Name = "BuildingServicesEngineering")]
        public IActionResult BuildingServicesEngineering()
        {
            return View("Subjects/BuildingServicesEngineering");
        }

        [Route("/students/subjects/design-development-engineering", Name = "DesignDevelopmentEngineering")]
        public IActionResult DesignDevelopmentEngineering()
        {
            return View("Subjects/DesignDevelopmentEngineering");
        }

        [Route("/students/subjects/design-surveying-planning", Name = "DesignSurveyingPlanning")]
        public IActionResult DesignSurveyingPlanning()
        {
            return View("Subjects/DesignSurveyingPlanning");
        }

        [Route("/students/subjects/digital-business-services", Name = "DigitalBusinessServices")]
        public IActionResult DigitalBusinessServices()
        {
            return View("Subjects/DigitalBusinessServices");
        }

        [Route("/students/subjects/digital-production-design-development", Name = "DigitalProductionDesignDevelopment")]
        public IActionResult DigitalProductionDesignDevelopment()
        {
            return View("Subjects/DigitalProductionDesignDevelopment");
        }

        [Route("/students/subjects/digital-support-services", Name = "DigitalSupportServices")]
        public IActionResult DigitalSupportServices()
        {
            return View("Subjects/DigitalSupportServices");
        }

        [Route("/students/subjects/education", Name = "Education")]
        public IActionResult Education()
        {
            return View("Subjects/Education");
        }

        [Route("/students/subjects/engineering-manufacturing-processing-control", Name = "EngineeringManufacturingProcessingControl")]
        public IActionResult EngineeringManufacturingProcessingControl()
        {
            return View("Subjects/EngineeringManufacturingProcessingControl");
        }

        [Route("/students/subjects/finance", Name = "Finance")]
        public IActionResult Finance()
        {
            return View("Subjects/Finance");
        }

        [Route("/students/subjects/health", Name = "Health")]
        public IActionResult Health()
        {
            return View("Subjects/Health");
        }

        [Route("/students/subjects/healthcare-science", Name = "HealthcareScience")]
        public IActionResult HealthcareScience()
        {
            return View("Subjects/HealthcareScience");
        }

        [Route("/students/subjects/maintenance-installation-repair", Name = "MaintenanceInstallationRepair")]
        public IActionResult MaintenanceInstallationRepair()
        {
            return View("Subjects/MaintenanceInstallationRepair");
        }

        [Route("/students/subjects/management-administration", Name = "ManagementAdministration")]
        public IActionResult ManagementAdministration()
        {
            return View("Subjects/ManagementAdministration");
        }

        [Route("/students/subjects/onsite-construction", Name = "OnsiteConstruction")]
        public IActionResult OnsiteConstruction()
        {
            return View("Subjects/OnsiteConstruction");
        }

        [Route("/students/subjects/science", Name = "Science")]
        public IActionResult Science()
        {
            return View("Subjects/Science");
        }

        [Route("/students/redirect", Name = "Redirect")]
        public IActionResult Redirect(RedirectViewModel viewModel)
        {
            var allowedUrls = _providerDataService
                    .GetWebsiteUrls();

            //Need to decode the url for comparison to the allow list,
            //as it has been encoded before being added to web pages
            var decodedUrl = WebUtility.UrlDecode(viewModel.Url);
            var targetUrl = 
                Url.IsLocalUrl(decodedUrl) || allowedUrls.ContainsKey(decodedUrl)
                ? viewModel.Url
                : "/students";

            return new RedirectResult(targetUrl, false);
        }

        [Route("/student")]
        public IActionResult IndexRedirect()
        {
            return RedirectToAction(nameof(Index));
        }

        [Route("/about", Name = "AboutOld")]
        public IActionResult AboutRedirect()
        {
            return RedirectToAction(nameof(About));
        }

        [Route("/why", Name = "WhyOld")]
        public IActionResult WhyRedirect()
        {
            return RedirectToAction(nameof(Why));
        }

        [Route("/subjects", Name = "SubjectsOld")]
        public IActionResult SubjectsRedirect()
        {
            return RedirectToAction(nameof(Subjects));
        }

        [Route("/find", Name = "FindOld")]
        public IActionResult FindRedirect()
        {
            return RedirectToAction(nameof(Find));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
