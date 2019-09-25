using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Models;

namespace sfa.Tl.Marketing.Communication.Controllers
{
    public class HelpController : Controller
    {

        [Route("cookies", Name = "Cookies")]
        public IActionResult Cookies()
        {
            return View();
        }

        [Route("accessibility", Name = "Accessibility")]
        public IActionResult Accessibility()
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
