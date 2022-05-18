using Microsoft.AspNetCore.Mvc;

namespace Selfy.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View();
        }


    }
}
