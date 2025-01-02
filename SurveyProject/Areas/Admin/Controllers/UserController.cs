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
        public async Task<IActionResult> Edit(string Id)
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

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, IdentityUserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(Id);
            if (existingUser == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(existingUser);
            Console.WriteLine("Current Roles: " + string.Join(", ", currentRoles));

            if (ModelState.IsValid)
            {
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;

                var oldRoles = await _userManager.GetRolesAsync(existingUser);
                foreach (var role in oldRoles)
                {
                    await _userManager.RemoveFromRoleAsync(existingUser, role);
                }

                var newRole = await _roleManager.FindByIdAsync(user.RoleId);
                if (newRole != null)
                {
                    await _userManager.AddToRoleAsync(existingUser, newRole.Name);

                    existingUser.RoleId = user.RoleId; // Assuming RoleId is a custom property in your IdentityUser
                }

               
                var updatedRoles = await _userManager.GetRolesAsync(existingUser);
                Console.WriteLine("Updated Roles: " + string.Join(", ", updatedRoles));

               
                var updateUserResult = await _userManager.UpdateAsync(existingUser);
                if (updateUserResult.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(existingUser);
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach (var error in updateUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(existingUser);
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
