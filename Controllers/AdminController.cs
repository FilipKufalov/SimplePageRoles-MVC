using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplePageRoles.Models;
using System.Data;

namespace SimplePageRoles.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AdminController(RoleManager<IdentityRole> roleManager,UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = roles.FirstOrDefault()
                });
            }
            return View(userViewModels);
        }


        [HttpPost]
        public async Task<IActionResult> AddRole(string userId, string userRole)
        {
            // Get the user from the database
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Add the role to the user
            var result = await _userManager.AddToRoleAsync(user, userRole);
            if (result.Succeeded)
            {
                // Role added successfully
                return RedirectToAction("Index");
            }
            else
            {
                // There was an error adding the role
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string userRole)
        {
            // Get the user from the database
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Remove the role to the user
            var result = await _userManager.RemoveFromRoleAsync(user, userRole);
            if (result.Succeeded)
            {
                // Role added successfully
                return RedirectToAction("Index");
            }
            else
            {
                // There was an error adding the role
                return BadRequest();
            }
        }


        //public async Task<IActionResult> AddRole(string roleName)
        //{

        //    var res = await _roleManager.CreateAsync(new IdentityRole(roleName));
        //    if (res.Succeeded)
        //    {
        //        return Ok("created successfully !");
        //    }
        //    return View();
        //}

        //public async Task<IActionResult> FindUser(string email)
        public async Task<IActionResult> FindUser()
        {
            var email = "contact@admin.com";
            var user = await _userManager.FindByEmailAsync(email);
            await _userManager.AddToRoleAsync(user, "Admin");

            // or 
            // var user = await _userManager.FindByNameAsync(email);
            // or 
            // var user = await _userManager.FindByIdAsync(email);

            // ...
            return View(user);
        }

    }
}
