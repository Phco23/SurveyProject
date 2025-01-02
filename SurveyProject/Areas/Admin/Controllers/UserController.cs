using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurveyProject.Models;
using SurveyProject.Models.ViewModels;
using SurveyProject.Repository;
using System.Security.Claims;

namespace SurveyProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;
        private SignInManager<IdentityUserModel> _signInManager;

        public UserController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUserModel> userManager, DataContext dataContext, SignInManager<IdentityUserModel> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _dataContext = dataContext;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index(string role)
        {
            var usersWithRoles = await (from u in _dataContext.Users
                                        join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                                        join r in _dataContext.Roles on ur.RoleId equals r.Id
                                        where r.Name != "Student" // Exclude "Student" role
                                        select new { User = u, RoleName = r.Name })
                                .AsNoTracking()
                                .ToListAsync();

            // Filter users by role if a role is provided
            if (!string.IsNullOrEmpty(role))
            {
                usersWithRoles = usersWithRoles.Where(u => u.RoleName == role).ToList();
            }

            // Group the filtered users by roles
            var groupedUsers = usersWithRoles
                .GroupBy(x => x.RoleName)
                .Select(g => new RoleGroupViewModel
                {
                    RoleName = g.Key,
                    Users = g.Select(u => new UserViewModel
                    {
                        Id = u.User.Id,
                        UserName = u.User.UserName,
                        Email = u.User.Email
                    }).ToList()
                })
                .ToList();

            ViewBag.SelectedRole = role; // To highlight the active role
            ViewBag.Roles = usersWithRoles.Select(u => u.RoleName).Distinct().ToList(); // For navigation

            return View(groupedUsers);
        }




        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userModel = new UserModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = new SelectList(roles, "Id", "Name", roles.FirstOrDefault(r => userRoles.Contains(r.Name))?.Id);

            return View(userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserModel model, string RoleId)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = model.UserName;
                user.Email = model.Email;

                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);

                    if (!string.IsNullOrEmpty(RoleId))
                    {
                        var newRole = await _roleManager.FindByIdAsync(RoleId);
                        if (newRole != null && !currentRoles.Contains(newRole.Name))
                        {
                            await _userManager.RemoveFromRolesAsync(user, currentRoles);
                            await _userManager.AddToRoleAsync(user, newRole.Name);
                        }
                    }

                    TempData["success"] = "User updated successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return View("Error");
            }
            TempData["success"] = "user delete ok";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new UserModel()); // Pass the correct model type to the view
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel model, string RoleId)
        {
            if (ModelState.IsValid)
            {
                var newUser = new IdentityUserModel
                {
                    UserName = model.UserName,
                    Email = model.Email
                };

                var createResult = await _userManager.CreateAsync(newUser, model.Password);

                if (createResult.Succeeded)
                {
                    if (!string.IsNullOrEmpty(RoleId))
                    {
                        var role = await _roleManager.FindByIdAsync(RoleId);
                        if (role != null)
                        {
                            await _userManager.AddToRoleAsync(newUser, role.Name);
                        }
                    }

                    TempData["success"] = "User created successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(model); // Return the correct model type to the view
        }



    }
}
