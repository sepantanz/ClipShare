using ClipShare.Core.Entities;
using ClipShare.Utility;
using ClipShare.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClipShare.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            var loginVM = new Login_vm
            {
                ReturnUrl = returnUrl,
            };

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login_vm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.ReturnUrl = model.ReturnUrl ?? Url.Content("~/");

            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                user = await userManager.FindByEmailAsync(model.UserName);
            }

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password. Please try again.");
                return View(model);
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded)
            {
                await HandleSignInUserAsync(user);
                return LocalRedirect(model.ReturnUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register_vm model)
        {
            if (ModelState.IsValid)
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("ConfirmPassword", "Confirm password does not match password.");

                    return View(model);
                }

                if (await CheckEmailExistsAsync(model.Email))
                {
                    ModelState.AddModelError("Email", $"Email address of {model.Email} is taken. Please try using another email address.");
                }

                if (await CheckNameExistsAsync(model.Email))
                {
                    ModelState.AddModelError("Name", $"Name of {model.Email} is taken. Please try another name.");
                }

                var userToAdd = new AppUser
                {
                    Name = model.Name,
                    UserName = model.Name.ToLower(),
                    Email = model.Email.ToLower(),
                };

                var result = await userManager.CreateAsync(userToAdd, model.Password);
                await userManager.AddToRoleAsync(userToAdd, SD.UserRole);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }

                await HandleSignInUserAsync(userToAdd);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Private Methods

        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await userManager.Users.AnyAsync(x => x.Email == email);
        }

        private async Task<bool> CheckNameExistsAsync(string name)
        {
            return await userManager.Users.AnyAsync(x => x.Name.ToLower() == name);
        }

        private async Task HandleSignInUserAsync(AppUser user)
        {
            var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, user.Name));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

            var roles = await userManager.GetRolesAsync(user);
            claimsIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var principal = new ClaimsPrincipal(claimsIdentity);

            // Using this method in order to assign identityClaims into User.Identity and sign the user in using built in dotnet identity
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        #endregion
    }
}
