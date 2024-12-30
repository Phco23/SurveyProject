using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyProject.Models;
using SurveyProject.Repository;

namespace SurveyProject.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<IdentityUserModel> _userManager;
        private SignInManager<IdentityUserModel> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        public AccountController(RoleManager<IdentityRole> roleManager, SignInManager<IdentityUserModel> signInManager, UserManager<IdentityUserModel> userManager)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;


		}

        public IActionResult Login(string returnUrl = "/")
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
		public async Task<IActionResult>  NewPass(string returnUrl)
		{
			return View();
		}
		public async Task<IActionResult>  ForgetPass(string returnUrl)
		{
			return View();
		}

        [HttpGet]
        public IActionResult AccessDenied(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel LoginVM)
        {

            if (ModelState.IsValid)
            {
                // Sign in using the provided credentials
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(LoginVM.UserName, LoginVM.Password, false, false);
               
                if (result.Succeeded)
                {
                    // Validate the return URL to prevent open redirect attacks

                    return Redirect(LoginVM.ReturnUrl ?? "/");


                }


                ModelState.AddModelError("", "Invalid username and password");
            }

            // Return the view with the login model if login failed
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
                    var role = await _roleManager.FindByNameAsync("Student");
                    if (role != null)
                    {
                        newUser.RoleId = role.Id; // Set RoleId manually
                        await _userManager.UpdateAsync(newUser); // Update user to save RoleId
                        await _userManager.AddToRoleAsync(newUser, "Student");
                    }

                    TempData["SuccessMessage"] = "Account pending for register success! We will soon send the result to your email.";

                    return View();
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
