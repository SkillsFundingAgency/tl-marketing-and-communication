﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Models;

namespace sfa.Tl.Marketing.Communication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Why()
        {
            return View();
        }

        public IActionResult Subjects()
        {
            return View();
        }

        public IActionResult Find(FindViewModel viewModel)
        {
            return View(viewModel);
        }

        public IActionResult Find2(FindViewModel viewModel)
        {
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}