using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Controllers;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    public class EmployerControllerBuilder
    {
        public EmployerController BuildEmployerController()
        {
            var controller = new EmployerController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            return controller;
        }
    }
}
