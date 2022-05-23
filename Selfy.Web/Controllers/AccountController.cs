using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Selfy.Core.Identity;
using Selfy.Data.Identity;
using Selfy.Web.ViewModels;
using System.Text;

namespace Selfy.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;   
            _signInManager = signInManager;
            CheckRoles();
        }
        private void CheckRoles()
        {
            foreach (string item in Roles.RoleList)
            {
                if (_roleManager.RoleExistsAsync(item).Result)
                {
                    continue;
                }
                var result = _roleManager.CreateAsync(new ApplicationRole()
                {
                    Name = item,
                }).Result;
            }
        }

        [HttpGet]
        public IActionResult Register() { return View(); }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Bir hata oluştu");
                return View(model);
            }

            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                Name = model.Name,
                Surname = model.Surname
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Bir hata oluştu");
                return View(model);
            }

            var count = _userManager.Users.Count();
            result = await _userManager.AddToRoleAsync(user, count == 1 ? Roles.Admin : Roles.Passive);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);

            //var emailMessage = new MailModel()
            //{
            //    To = new List<EmailModel> { new EmailModel()
            //{
            //    Address = user.Email,
            //    Name = user.Name
            //}},
            //    Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here </a>.",
            //    Subject = "Confirm your email"
            //};

            //await _emailService.SendMailAsync(emailMessage);



            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var model = new LoginViewModel()
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);

            if (result.Succeeded)
            {
                var user = _userManager.FindByNameAsync(model.UserName).Result;
                HttpContext.Session.SetString("User", System.Text.Json.JsonSerializer.Serialize(new
                {
                    user.Name,
                    user.Surname,
                    user.Email
                }));

                //model.ReturnUrl = string.IsNullOrEmpty(model.ReturnUrl) ? "~/" : model.ReturnUrl;

                //model.ReturnUrl = model.ReturnUrl ?? Url.Action("Index", "Home");

                model.ReturnUrl ??= Url.Content("~/");

                return LocalRedirect(model.ReturnUrl);
            }
            else if (result.IsLockedOut)
            {

            }
            else if (result.RequiresTwoFactor)
            {

            }

            ModelState.AddModelError(string.Empty, "Username or password is incorrect");
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ConfirmResetPassword(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return BadRequest("Hatalı istek");
            }

            ViewBag.Code = code;
            ViewBag.UserId = userId;
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var name = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(name);
            var model = new UpdateProfilePasswordViewModel
            {
                UserProfileVM = new UserProfileViewModel()
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    RegisterDate = user.RegisterDate
                }
            };

            return View(model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(UpdateProfilePasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var name = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(name);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found!");
                return View(model);
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
            if (user.Email != model.UserProfileVM.Email && !isAdmin)
            {
                await _userManager.RemoveFromRoleAsync(user, Roles.User);
                await _userManager.AddToRoleAsync(user, Roles.Passive);
                user.EmailConfirmed = false;

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);

                //var emailMessage = new MailModel()
                //{
                //    To = new List<EmailModel> { new()
                //{
                //    Address = model.UserProfileVM.Email,
                //    Name = model.UserProfileVM.Name
                //}},
                //    Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here </a>.",
                //    Subject = "Confirm your email"
                //};

                //await _emailService.SendMailAsync(emailMessage);
            }


            user.Name = model.UserProfileVM.Name;
            user.Surname = model.UserProfileVM.Surname;
            user.Email = model.UserProfileVM.Email;
            //user.UserName = model.UserProfileVM.UserName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                ViewBag.Message = "Your profile has been updated successfully";
                var userl = await _userManager.FindByNameAsync(user.UserName);
                await _signInManager.SignInAsync(userl, true);
                HttpContext.Session.SetString("User", System.Text.Json.JsonSerializer.Serialize(new
                {
                    user.Name,
                    user.Surname,
                    user.Email
                }));
            }
            else
            {
                var message = string.Join("<br>", result.Errors.Select(x => x.Description));
                ViewBag.Message = message;
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        

       

       

    }
}
