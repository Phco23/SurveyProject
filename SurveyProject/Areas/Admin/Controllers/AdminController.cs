
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
                var subject = "Account Approved";
                var body = $@"
                    <div style='font-family: Arial, sans-serif; background-color: #f4f7fa; padding: 20px;'>
                        <div class='container' style='max-width: 600px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); padding: 30px;'>
                            <h2 style='color: #007bff; text-align: center;'>Account Approved</h2>
                            <p style='font-size: 16px; color: #555;'>Dear {user.UserName},</p>
        
                            <p style='font-size: 16px; color: #555;'>We're excited to let you know that your account has been successfully approved! 🎉</p>
        
                            <p style='font-size: 16px; color: #555;'>To get started, simply click the link below to log in:</p>
        
                            <div class='text-center'>
                                <a href='https://localhost:7072/Account/Login' class='btn btn-primary' style='font-size: 16px; padding: 12px 24px; text-decoration: none; border-radius: 5px;'>
                                    Login Now
                                </a>
                            </div>
        
                            <p style='font-size: 16px; color: #555; margin-top: 20px;'>If you encounter any issues, feel free to reach out to us. We're here to help!</p>
        
                            <p style='font-size: 16px; color: #555;'>Best regards,</p>
                            <p style='font-size: 16px; color: #555; font-weight: bold;'>The eLearning Team</p>
        
                            <hr style='border: 1px solid #f1f1f1;' />
                            <p style='font-size: 12px; color: #888; text-align: center;'>If you didn't request this, please ignore this email.</p>
                        </div>
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
                    var subject = "Account Rejected";
                    var body = $@"
                        <div style='font-family: Arial, sans-serif; background-color: #f4f7fa; padding: 20px;'>
                            <div class='container' style='max-width: 600px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); padding: 30px;'>
                                <h2 style='color: #dc3545; text-align: center;'>Account Rejected</h2>
                                <p style='font-size: 16px; color: #555;'>Dear {user.UserName},</p>
        
                                <p style='font-size: 16px; color: #555;'>We regret to inform you that your account application has been rejected. 😔</p>
        
                                <p style='font-size: 16px; color: #555;'>We understand this may be disappointing, but we encourage you to review your application and ensure all required information is provided. If you'd like to reapply, you are welcome to do so at any time.</p>
        
                                <p style='font-size: 16px; color: #555;'>If you believe there has been an error, please don't hesitate to contact us for further clarification or assistance.</p>
        
                                <div class='text-center'>
                                    <a href='https://localhost:7072/Contact' class='btn btn-danger' style='font-size: 16px; padding: 12px 24px; text-decoration: none; border-radius: 5px;'>
                                        Contact Support
                                    </a>
                                </div>
        
                                <p style='font-size: 16px; color: #555; margin-top: 20px;'>We appreciate your understanding and hope to have the opportunity to assist you in the future.</p>
        
                                <p style='font-size: 16px; color: #555;'>Best regards,</p>
                                <p style='font-size: 16px; color: #555; font-weight: bold;'>The eLearning Team</p>
        
                                <hr style='border: 1px solid #f1f1f1;' />
                                <p style='font-size: 12px; color: #888; text-align: center;'>If you have any questions or concerns, please feel free to reach out to us at any time.</p>
                            </div>
                        </div>
                        ";



                    await _emailService.SendEmailAsync(user.Email, subject, body);
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
                    // return RedirectToAction("Index", "Home");
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
