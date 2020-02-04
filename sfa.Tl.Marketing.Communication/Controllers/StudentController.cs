using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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

        [Route("/student", Name = "Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/student/about", Name = "About")]
        public IActionResult About()
        {
            return View();
        }

        [Route("/student/why", Name = "Why")]
        public IActionResult Why()
        {
            return View();
        }

        [Route("/student/subjects", Name = "Subjects")]
        public IActionResult Subjects()
        {
            return View();
        }


        [Route("/student/find", Name = "Find")]
        public IActionResult Find(FindViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Postcode))
                viewModel.ShouldSearch = true;

            viewModel.MapApiKey = _configuration.GoogleMapsApiKey;

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
