
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LibraryDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        

        public AccountController(UserManager<ApplicationUser> userManager, LibraryDbContext context, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            

        }

        [HttpGet]
        
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var studentRole = _context.Roles.FirstOrDefault(r => r.RoleName == "Student");

                if (studentRole == null)
                {
                    ModelState.AddModelError("", "Student role not found in database.");
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    StudentId = model.IdentificationNumber,
                    RoleId = studentRole.Id 
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user != null)
                    {
                        if (await _userManager.IsInRoleAsync(user, "Student"))
                        {
                            return RedirectToAction("Index", "StudentDashboard");
                        }

                        return RedirectToAction("Dashboard", "Account"); 
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }



        public async Task<IActionResult> Logout()
        {
         
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
        
        public IActionResult Dashboard()
        {
            

        var totalBooks = _context.Books.Count();
        var availableBooks = _context.Books.Sum(b => b.AvailableCopies);
        var borrowedBooks = _context.Loans.Count();
        var overdueBooks = _context.Loans.Count(l => l.DueDate < DateTime.Now && l.ReturnDate == null);
        var pastPapers = _context.PastPapers.Count();

        var model = new DashboardViewModel
           {
            TotalBooks = totalBooks,
            AvailableBooks = availableBooks,
            BorrowedBooks = borrowedBooks,
            OverdueBooks = overdueBooks,
            PastPapers = pastPapers
         };


            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {


                    return View("ResetPassword", "Account");

                }

            }

            return View();

        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();

        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {

            return View();
        }
    }
}