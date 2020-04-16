using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Controllers
{
    public class StudentController : Controller
    {
        private readonly ConfigurationOptions _configuration;
        private readonly IProviderSearchService _providerSearchService;
        private readonly IMapper _mapper;

        public StudentController(ConfigurationOptions configuration, IProviderSearchService providerSearchService, IMapper mapper)
        {
            _configuration = configuration;
            _providerSearchService = providerSearchService;
            _mapper = mapper;
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

        [Route("/students/find", Name = "Find")]
        public async Task<IActionResult> Find(FindViewModel viewModel)
        {
            viewModel.ShouldSearch = !string.IsNullOrEmpty(viewModel.Postcode);

            var qualifications = _providerSearchService.GetQualifications();
            var qualificationId = viewModel.SelectedQualificationId ?? 0;
            var qualificationSelectListItems = qualifications.Select(q => new SelectListItem { Text = q.Name, Value = q.Id.ToString(), Selected = (q.Id == qualificationId) });
            viewModel.Qualifications = qualificationSelectListItems;

            if (!string.IsNullOrEmpty(viewModel.Postcode))
            {
                var numberOfItems = viewModel.NumberOfItems.HasValue? viewModel.NumberOfItems.Value + 5 : 5;
                viewModel.NumberOfItems = numberOfItems;

                var searchRequest = new SearchRequest { Postcode = viewModel.Postcode, NumberOfItems = viewModel.NumberOfItems.Value, QualificationId = qualificationId };
                var searchResults = await _providerSearchService.Search(searchRequest);
                var providerViewModels = _mapper.Map<IEnumerable<ProviderLocationViewModel>>(searchResults);
                viewModel.ProviderLocations = providerViewModels;
            }

            return View(viewModel);
        }

        [Route("/students/subjects/design-surveying-planning", Name = "DesignSurveyingPlanning")]
        public IActionResult DesignSurveyingPlanning()
        {
            return View("Subjects/DesignSurveyingPlanning");
        }

        [Route("/students/subjects/digital-production-design-development", Name = "DigitalProductionDesignDevelopment")]
        public IActionResult DigitalProductionDesignDevelopment()
        {
            return View("Subjects/DigitalProductionDesignDevelopment");
        }

        [Route("/students/subjects/education", Name = "Education")]
        public IActionResult Education()
        {
            return View("Subjects/Education");
        }

        [Route("/students/subjects/building-services-engineering", Name = "BuildingServicesEngineering")]
        public IActionResult BuildingServicesEngineering()
        {
            return View("Subjects/BuildingServicesEngineering");
        }

        [Route("/students/subjects/digital-business-services", Name = "DigitalBusinessServices")]
        public IActionResult DigitalBusinessServices()
        {
            return View("Subjects/DigitalBusinessServices");
        }

        [Route("/students/subjects/digital-support-services", Name = "DigitalSupportServices")]
        public IActionResult DigitalSupportServices()
        {
            return View("Subjects/DigitalSupportServices");
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
        public void Redirect(RedirectViewModel viewModel)
        {
            Response.Redirect(viewModel.Url, false);
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
