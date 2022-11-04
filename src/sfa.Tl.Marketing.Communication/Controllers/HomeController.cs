using Microsoft.AspNetCore.Mvc;

namespace sfa.Tl.Marketing.Communication.Controllers;

public class HomeController : Controller
{
    [Route("/", Name = "Landing")]
    public IActionResult Landing()
    {
        return View();
    }
}