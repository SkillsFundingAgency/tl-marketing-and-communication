using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using sfa.Tl.Marketing.Communication.Models;

namespace sfa.Tl.Marketing.Communication.Controllers
{
    public class StudentController : Controller
    {
        private readonly ConfigurationOptions _configuration;

        public StudentController(ConfigurationOptions configuration)
        {
            _configuration = configuration;
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
        public IActionResult Find(FindViewModel viewModel)
        {
            var postcode = TempData["Postcodet"] as string;
            var qualification = TempData["Qualificationt"] as string;

            viewModel.Postcode = postcode ?? viewModel.Postcode;
            viewModel.Qualification = qualification ?? viewModel.Qualification;
            
            viewModel.ShouldSearch = !string.IsNullOrEmpty(viewModel.Postcode);

            viewModel.MapApiKey = _configuration.GoogleMapsApiKey;

            return View(viewModel);
        }

        [Route("/students/redirect", Name = "Redirect")]
        public void Redirect(RedirectViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Postcode))
            {
                TempData["Postcodet"] = viewModel.Postcode;
                TempData["Qualificationt"] = viewModel.Qualification;
            }

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
