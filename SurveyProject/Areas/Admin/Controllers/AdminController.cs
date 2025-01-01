
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SurveyProject.Repository;
using eLearning.Repository;


namespace SurveyProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUserModel> _userManager;
        private readonly EmailService _emailService;

        private readonly DataContext _context;
        public AdminController(UserManager<IdentityUserModel> userManager, DataContext context, EmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            
            _emailService = emailService;
        }

        public async Task<IActionResult> PendingApprovals()
        {
            var users = await _userManager.Users
                .Where(u => !u.IsApproved) // loc tai khoan chx ddc duyet
                .ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsApproved = true; // duyet tai khoan
                await _userManager.UpdateAsync(user); // cap nhat thong tin ng dung
                var subject = "Your Account is Under Review";
                var body = $@"
                                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: auto; border: 1px solid #ddd; border-radius: 10px; padding: 20px; background-color: #f9f9f9;'>
                                    <h2 style='color: #06BBCC; text-align: center;'>Your Account is Under Review</h2>
                                    <p>Dear <strong>{user.UserName}</strong>,</p>
                                    <p>Thank you for registering. Your account is currently under review.</p>
                                    <p>We will notify you about the result as soon as possible via email.</p>
                                    <p style='margin-top: 30px;'>Best regards,<br>eLearning</p>
                                </div>
                            ";

                await _emailService.SendEmailAsync(user.Email, subject, body);
                return RedirectToAction("PendingApprovals");
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> RejectUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user); // xoa tai khoan
                return RedirectToAction("PendingApprovals");
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> EditProfiles(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound(); 
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(); 
            }

            return View(user); 
        }


        [HttpPost]
        public async Task<IActionResult> EditProfiles(IdentityUserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.IsApproved = model.IsApproved;
                user.UserName = model.UserName;
                user.Email = model.Email;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("PendingApprovals"); 
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users
                .Where(u => u.IsApproved == true) 
                .ToListAsync();

            return View(users); 
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound();

            return View(user);  
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
            }

            return RedirectToAction("ManageUsers");
        }




    }

}
