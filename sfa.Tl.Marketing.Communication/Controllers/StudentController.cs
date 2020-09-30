using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;

namespace sfa.Tl.Marketing.Communication.Controllers
{
    public class StudentController : Controller
    {
        public const string AllowedRedirectUrlsCacheKey = "Allowed_Redirect_Urls";
        public const int CacheExpiryInMinutes = 120;

        private readonly IMemoryCache _cache;
        private readonly IProviderDataService _providerDataService;
        private readonly IProviderSearchEngine _providerSearchEngine;

        public StudentController(
            IProviderDataService providerDataService, 
            IProviderSearchEngine providerSearchEngine, 
            IMemoryCache cache)
        {
            _providerSearchEngine = providerSearchEngine ?? throw new ArgumentNullException(nameof(providerSearchEngine));
            _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
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
            var searchResults = await _providerSearchEngine.Search(viewModel);
            return View(searchResults);
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
        public IActionResult Redirect(RedirectViewModel viewModel)
        {
            if (!_cache.TryGetValue(AllowedRedirectUrlsCacheKey, out HashSet<string> allowedUrls))
            {
                allowedUrls = new HashSet<string>(_providerDataService.GetWebsiteUrls());

                _cache.Set(AllowedRedirectUrlsCacheKey, allowedUrls,
                    new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(CacheExpiryInMinutes)));
            }

            var targetUrl = Url.IsLocalUrl(viewModel.Url) || allowedUrls.Contains(viewModel.Url)
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
