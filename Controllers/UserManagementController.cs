using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{

    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LibraryDbContext _context;



        public UserManagementController(UserManager<ApplicationUser> userManager, LibraryDbContext context)
        {

            _userManager = userManager;
            _context = context;
        }


        public IActionResult Index()
        {
            var users = _userManager.Users
         .Select(u => new UserListViewModel
         {
             FullName = u.FullName,
             Email = u.Email,
             PhoneNumber = u.PhoneNumber,
             Role = _context.Roles
                    .Where(r => r.Id == u.RoleId)
                    .Select(r => r.RoleName)
                    .FirstOrDefault()
         }).ToList();


            var roles = _context.Roles
                .Select(r => new CreateRoleViewModel
                {
                    RoleName = r.RoleName
                }).ToList();

            var viewModel = new UserAndRoleViewModel
            {
                Users = users,
                Roles = roles
            };

            return View(viewModel);
        }




        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var selectedRole = _context.Roles.FirstOrDefault(r => r.RoleName == model.Roles);

                if (selectedRole == null)
                {
                    ModelState.AddModelError("", "Selected role does not exist.");
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    RoleId = selectedRole.Id 
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    
                    await _userManager.AddToRoleAsync(user, selectedRole.RoleName);

                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult EditUser()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserListViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == model.Role);
            if (role != null)
            {
                user.RoleId = role.Id;
            }


            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return RedirectToAction("Index");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult DeleteUser()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
            }

            return NotFound();
        }

        [HttpGet]
        public IActionResult CreateRole()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoleName == model.RoleName);

                if (existingRole != null)
                {
                    ModelState.AddModelError("", "Role already exists.");
                    return View(model);
                }

                var role = new Roles
                {
                    Id = Guid.NewGuid().ToString(),
                    RoleName = model.RoleName
                };

                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditRole(string OldRoleName, string NewRoleName)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == OldRoleName);
            if (role == null)
                return NotFound();

            role.RoleName = NewRoleName;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return NotFound();
        }


    }
}








         
