
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SurveyProject.Repository;


namespace SurveyProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUserModel> _userManager;
        private readonly DataContext _context;
        public AdminController(UserManager<IdentityUserModel> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
            
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
