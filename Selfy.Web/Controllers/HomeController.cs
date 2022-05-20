using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Selfy.Core.Identity;
using Selfy.Data.Identity;

namespace Selfy.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
           
        }

        public IActionResult Index()
        {
            return View();
        }


    }
}
