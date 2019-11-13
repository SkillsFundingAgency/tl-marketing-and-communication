using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Models;

namespace sfa.Tl.Marketing.Communication.Controllers
{
    public class ABController : Controller
    {

        [Route("/baseline", Name = "Baseline")]
        public IActionResult Baseline()
        {
            return View();
        }


        [Route("/v1", Name = "Variant1")]
        public IActionResult Variant1()
        {
            return View();
        }

        [Route("/v2", Name = "Variant2")]
        public IActionResult Variant2()
        {
            return View();
        }

        [Route("/v3", Name = "Variant3")]
        public IActionResult Variant3()
        {
            return View();
        }

        [Route("/v4", Name = "Variant4")]
        public IActionResult Variant4()
        {
            return View();
        }

        [Route("/v5", Name = "Variant5")]
        public IActionResult Variant5()
        {
            return View();
        }

        [Route("/v6", Name = "Variant6")]
        public IActionResult Variant6()
        {
            return View();
        }

        [Route("/v7", Name = "Variant7")]
        public IActionResult Variant7()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
