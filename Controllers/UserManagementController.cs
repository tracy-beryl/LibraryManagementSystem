using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly LibraryDbContext _context;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            LibraryDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // INDEX

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .Where(u => u.IsActive)
                .ToListAsync();

            var userViewModels = users.Select(async u => new UserListViewModel
            {
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = (await _userManager.GetRolesAsync(u)).FirstOrDefault()
            }).Select(t => t.Result).ToList();

            var roles = await _roleManager.Roles
                .Select(r => new CreateRoleViewModel
                {
                    RoleName = r.Name
                }).ToListAsync();

            var viewModel = new UserAndRoleViewModel
            {
                Users = userViewModels,
                Roles = roles
            };

            return View(viewModel);
        }

       
        // CREATE USER

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!await _roleManager.RoleExistsAsync(model.Roles))
            {
                ModelState.AddModelError("", "Selected role does not exist.");
                return View(model);
            }

            if (model.Roles == "Student" && string.IsNullOrWhiteSpace(model.AdmissionNumber))
            {
                ModelState.AddModelError("AdmissionNumber", "Admission number is required for students.");
                return View(model);
            }

            if ((model.Roles == "Librarian" || model.Roles == "Lecturer") && string.IsNullOrWhiteSpace(model.StaffNumber))
            {
                ModelState.AddModelError("StaffNumber", "Staff number is required.");
                return View(model);
            }

            if ((model.Roles == "Librarian" || model.Roles == "Lecturer") && string.IsNullOrWhiteSpace(model.Department))
            {
                ModelState.AddModelError("Department", "Department is required.");
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.FullName) ||
    model.FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 2)
            {
                ModelState.AddModelError("FullName", "Please enter at least two names.");
                return View(model);
            }

            var names = model.FullName
             .Trim()
             .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var firstName = names.FirstOrDefault();
            var lastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "";

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName.Trim(),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = model.PhoneNumber,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, model.Roles);

            if (model.Roles == "Student")
            {
                var studentProfile = new StudentProfile
                {
                    UserId = user.Id,
                    AdmissionNumber = model.AdmissionNumber.Trim()
                };
                _context.StudentProfiles.Add(studentProfile);
            }
            else if (model.Roles == "Librarian" || model.Roles == "Lecturer")
            {
                var staffProfile = new StaffProfile
                {
                    UserId = user.Id,
                    StaffNumber = model.StaffNumber.Trim(),
                    Department = model.Department.Trim()
                };
                _context.StaffProfiles.Add(staffProfile);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        // EDIT USER

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserListViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            var user = await _userManager.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.StaffProfile)
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
                return NotFound();

            // Validate full name has at least two names
            if (string.IsNullOrWhiteSpace(model.FullName) ||
                model.FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 2)
            {
                TempData["Error"] = "Please enter at least two names.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(model.Role))
            {
                TempData["Error"] = "Role is required.";
                return RedirectToAction("Index");
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                TempData["Error"] = "Selected role does not exist.";
                return RedirectToAction("Index");
            }

            user.FullName = model.FullName.Trim();
            user.PhoneNumber = model.PhoneNumber;

            // Split full name into first and last name
            var names = user.FullName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            user.FirstName = names.FirstOrDefault();
            user.LastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "";

            // Update roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, model.Role);

            // Save changes
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
                return RedirectToAction("Index");
            }

            TempData["Success"] = "User updated successfully.";
            return RedirectToAction("Index");
        }


        // DEACTIVATE USER

        [HttpPost]
        public async Task<IActionResult> DeactivateUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound();

            user.IsActive = false;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return RedirectToAction("Index");

            return NotFound();
        }

        
        // CREATE ROLE
        
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _roleManager.RoleExistsAsync(model.RoleName))
            {
                ModelState.AddModelError("", "Role already exists.");
                return View(model);
            }

            var result = await _roleManager.CreateAsync(
                new IdentityRole(model.RoleName));

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            return RedirectToAction("Index");
        }

        
        // EDIT ROLE

        [HttpPost]
        public async Task<IActionResult> EditRole(string oldRoleName, string newRoleName)
        {
            var role = await _roleManager.FindByNameAsync(oldRoleName);
            if (role == null)
                return NotFound();

            role.Name = newRoleName;
            role.NormalizedName = newRoleName.ToUpper();

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                return NotFound();

            return RedirectToAction("Index");
        }

       
        // DELETE ROLE
        
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound();

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
                return NotFound();

            return RedirectToAction("Index");
        }
    }
}