using ITPortal.AuthServer.Application;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITPortal.AuthServer.Web.Entry.Controllers
{
    [AppAuthorize]
    public class HomeController : Controller
    {
        public HomeController()
        {
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewBag.Description = "";

            return View();
        }
        public IActionResult Login()
        {
            ViewBag.Description = "";

            return View("../Account/Login");
        }
    }
}
