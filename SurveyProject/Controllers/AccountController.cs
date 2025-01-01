using eLearning.Repository;
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
        private readonly EmailService _emailService;

        public AccountController(RoleManager<IdentityRole> roleManager, SignInManager<IdentityUserModel> signInManager, UserManager<IdentityUserModel> userManager, EmailService emailService )
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;

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
                    var subject = "Account Registration Pending";
                    var body = $@"
                        <div style='font-family: Arial, sans-serif; background-color: #f4f7fa; padding: 20px;'>
                            <div class='container' style='max-width: 600px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); padding: 30px;'>
                                <h2 style='color: #007bff; text-align: center;'>Account Registration Pending</h2>
                                <p style='font-size: 16px; color: #555;'>Dear {user.UserName},</p>
        
                                <p style='font-size: 16px; color: #555;'>Thank you for registering with us! We have received your registration request, and it is currently under review.</p>
        
                                <p style='font-size: 16px; color: #555;'>Once your account has been approved, you will receive another email with instructions on how to log in and get started.</p>
        
                                <p style='font-size: 16px; color: #555;'>In the meantime, please feel free to explore our site, and don’t hesitate to reach out if you have any questions.</p>
        
                                <div class='text-center'>
                                    <a href='https://localhost:7072/ContactUs' class='btn btn-primary' style='font-size: 16px; padding: 12px 24px; text-decoration: none; border-radius: 5px;'>
                                        Contact Us
                                    </a>
                                </div>
        
                                <p style='font-size: 16px; color: #555; margin-top: 20px;'>Best regards,</p>
                                <p style='font-size: 16px; color: #555; font-weight: bold;'>The eLearning Team</p>
        
                                <hr style='border: 1px solid #f1f1f1;' />
                                <p style='font-size: 12px; color: #888; text-align: center;'>If you have any questions, please don’t hesitate to reach out to our support team.</p>
                            </div>
                        </div>

                    ";

                    await _emailService.SendEmailAsync(user.Email, subject, body);
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
