using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyProject.Models;
using System.Diagnostics;

namespace SurveyProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUserModel> _userManager;
        private readonly SignInManager<IdentityUserModel> _signInManager;


        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUserModel> userManager, SignInManager<IdentityUserModel> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;

        }


        public IActionResult ResponseFeedback()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public async Task<IActionResult> EditProfiles(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return NotFound(); 
                }
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(); 
            }

            return View(user); 
        }

        [HttpPost]
        public async Task<IActionResult> EditProfiles(IdentityUserModel model, string currentPassword, string newPassword)
        {
            if (!ModelState.IsValid)
            {
                return View(model); 
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName; 
            user.Email = model.Email; 
            user.IsApproved = model.IsApproved;

            //thay doi mk
            if (!string.IsNullOrEmpty(newPassword))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }
            //luuu thong tin ng dung
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                var signInResult = await _signInManager.PasswordSignInAsync(user, newPassword, false, false);
                if (signInResult.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully.";
                    return RedirectToAction("EditProfiles", new { userId = model.Id });
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }



    }
}
