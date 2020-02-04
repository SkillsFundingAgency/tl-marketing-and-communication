﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Models;

namespace sfa.Tl.Marketing.Communication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ConfigurationOptions _configuration;

        public HomeController(ConfigurationOptions configuration)
        {
            _configuration = configuration;
        }

        [Route("/", Name = "Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/Home", Name = "Home")]
        public IActionResult Home()
        {
            return RedirectToAction(nameof(Index));
        }

        [Route("about", Name = "About")]
        public IActionResult About()
        {
            return View();
        }

        [Route("why", Name = "Why")]
        public IActionResult Why()
        {
            return View();
        }

        [Route("subjects", Name = "Subjects")]
        public IActionResult Subjects()
        {
            return View();
        }


        [Route("landing", Name = "Landing")]
        public IActionResult Landing()
        {
            return View();
        }


        [Route("find", Name = "Find")]
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
