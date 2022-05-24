using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Selfy.Business.Services;
using Selfy.Core.Emails;
using Selfy.Core.Identity;
using Selfy.Data.Identity;
using Selfy.Web.ViewModels;
using System.Text;
using System.Text.Encodings.Web;

namespace Selfy.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IEmailService emailService, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
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
        public IActionResult Register() 
        { 
            return View(); 
        }

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
                Surname = model.Surname,
                PhoneNumber = model.Phone
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

            var emailMessage = new MailModel()
            {
                To = new List<EmailModel> { new EmailModel()
            {
                Adress = user.Email,
                Name = user.Name
            }},
                Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here </a>.",
                Subject = "Confirm your email"
            };

            await _emailService.SendMailAsync(emailMessage);



            return RedirectToAction("Login");
        }


        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound($"Unable to load user with ID ${userId}");

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            ViewBag.StatusMessage = result.Succeeded
                ? "Thank you for confirming your email"
                : "Error confirming your email.";

            if (!result.Succeeded || !_userManager.IsInRoleAsync(user, Roles.Passive).Result) return View();

            await _userManager.RemoveFromRoleAsync(user, Roles.Passive);
            await _userManager.AddToRoleAsync(user, Roles.User);

            return View();
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

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action("ConfirmResetPassword", "Account", new { userId = user.Id, code },
                    Request.Scheme);


                var emailMessage = new MailModel()
                {
                    To = new List<EmailModel>
                {
                    new EmailModel()
                    {
                        Adress = user.Email,
                        Name = user.Name
                    }
                },
                    Body =
                        $"You can chance your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here </a>.",
                    Subject = "Reset your password"
                };

                await _emailService.SendMailAsync(emailMessage);
            }

            ViewBag.Message = "Eğer mail adresiniz doğru ise şifre güncelleme yönergemiz gönderilmiştir";
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
        
        [HttpPost]
        public async Task<IActionResult> ConfirmResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı");
                return View(model);
            }

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));

            var result = await _userManager.ResetPasswordAsync(user, code, model.NewPassword);

            if (result.Succeeded)
            {
                var emailMessage = new MailModel()
                {
                    To = new List<EmailModel>
                {
                    new EmailModel()
                    {
                        Adress = user.Email,
                        Name = user.Name
                    }
                },
                    Body =
                        $"Your password has changed. You can login by <a href='{Url.Action("Login", "Account")}'>here</a>",
                    Subject = "Your password changed successfully"
                };
                await _emailService.SendMailAsync(emailMessage);
                TempData["Message"] = "Şifre değişikliğiniz gerçekleştirilmiştir";
                return RedirectToAction("Login");
            }

            var message = string.Join("<br>", result.Errors.Select(x => x.Description));
            TempData["Message"] = message;
            return RedirectToAction("Login");
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

                var emailMessage = new MailModel()
                {
                    To = new List<EmailModel>
            {
                new EmailModel()
                {
                    Adress = user.Email,
                    Name = user.Name
                }
            },
                    Body =
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here </a>.",
                    Subject = "Confirm your email"
                };

                await _emailService.SendMailAsync(emailMessage);
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
        
        
        [HttpPost, Authorize]
        public async Task<IActionResult> ChangePassword(UpdateProfilePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["PassError"] = "There has been an error.";
                return RedirectToAction(nameof(Profile));
            }

            var name = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(name);
            var result = await _userManager.ChangePasswordAsync(user, model.ChangePasswordVM.CurrentPassword,
                model.ChangePasswordVM.NewPassword);

            if (result.Succeeded)
            {
                TempData["PassSuccess"] = "Your password has been changed successfully";
            }
            else
            {
                var message = string.Join("<br>", result.Errors.Select(x => x.Description));
                TempData["PassError"] = message;
            }


            return RedirectToAction(nameof(Profile));
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        
    }
}
