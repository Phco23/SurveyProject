using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyProject.Models;

namespace SurveyProject.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<IdentityUserModel> _userManager;
        private SignInManager<IdentityUserModel> _signInManager;
        public AccountController(SignInManager<IdentityUserModel> signInManager, UserManager<IdentityUserModel> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel LoginVM)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(LoginVM.UserName, LoginVM.Password, false, false);
                if (result.Succeeded)
                {
                    return Redirect(LoginVM.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", " UserName and Password bị sai ");

            }
            return View(LoginVM);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                IdentityUserModel newUser = new IdentityUserModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                };

                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Tạo user thành công.";

                    return Redirect("/account/login");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
    }

}
