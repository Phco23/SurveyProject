using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace SurveyProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUserModel> _userManager;

        public AdminController(UserManager<IdentityUserModel> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> PendingApprovals()
        {
            var users = await _userManager.Users
                .Where(u => !u.IsApproved) // Lọc các tài khoản chưa được duyệt
                .ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsApproved = true; // Duyệt tài khoản
                await _userManager.UpdateAsync(user); // Cập nhật thông tin người dùng
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
                await _userManager.DeleteAsync(user); // Xóa tài khoản
                return RedirectToAction("PendingApprovals");
            }
            return NotFound();
        }
    }

}
