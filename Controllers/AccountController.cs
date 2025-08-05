using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
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

        public async Task<IActionResult> Dashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var firstName = currentUser?.FirstName ?? "User";
            var today = DateTime.Today;

            var totalBooks = _context.Books.Count();
            var activeLoans = _context.Loans.Where(l => l.ReturnDate == null).ToList();
            var overdueBooks = activeLoans.Count(l => l.DueDate < DateTime.Now);
            var pastPapers = _context.PastPapers.Count();
            var users = _context.Users.ToList();
            var books = _context.Books.Include(b => b.Loans).ToList();

            // Calculate available copies
            foreach (var book in books)
            {
                var borrowed = book.Loans.Count(l => l.ReturnDate == null);
                book.AvailableCopies = book.TotalCopies - borrowed;
            }

            var availableBooks = books.Sum(b => b.AvailableCopies);
            var borrowedBooks = activeLoans.Count;
            var totalVisitorsToday = users.Count(u => u.LastLoginDate.HasValue && u.LastLoginDate.Value.Date == today);
            var newMembers = users.Count(u => u.CreatedAt >= DateTime.Now.AddDays(-3));
            var topBooks = books.OrderByDescending(b => b.Loans.Count).Take(5).ToList();

            var visitorStats = Enumerable.Range(0, 7)
                .Select(i =>
                {
                    var date = today.AddDays(-i);
                    var count = users.Count(u => u.LastLoginDate.HasValue && u.LastLoginDate.Value.Date == date);
                    return new VisitorStat { Date = date, Count = count };
                })
                .Reverse().ToList();

            var categoryData = books
                .GroupBy(b => string.IsNullOrEmpty(b.Category) ? "Uncategorized" : b.Category)
                .Select(g => new CategoryData
                {
                    Category = g.Key,
                    AvailableCopies = g.Sum(b => b.AvailableCopies)
                })
                .ToList();

            var model = new DashboardViewModel
            {
                FirstName = firstName,
                TotalBooks = totalBooks,
                AvailableBooks = availableBooks,
                BorrowedBooks = borrowedBooks,
                OverdueBooks = overdueBooks,
                PastPapers = pastPapers,
                TotalVisitors = totalVisitorsToday,
                NewMembers = newMembers,
                Users = users,
                Books = books,
                Loans = activeLoans,
                TopBooks = topBooks,
                VisitorStats = visitorStats,
                CategoryChartData = categoryData
            };

            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
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
                    IdentificationNumber = model.IdentificationNumber,
                    RoleId = studentRole.Id,
                    CreatedAt = DateTime.Now //  Track when the user was created
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
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }

                if (!user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Your account is deactivated. Please contact the administrator.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Track last login
                    user.LastLoginDate = DateTime.Now;
                    await _userManager.UpdateAsync(user);

                    var studentRole = _context.Roles.FirstOrDefault(r => r.RoleName == "Student");

                    if (user.RoleId == studentRole?.Id)
                    {
                        return RedirectToAction("Index", "StudentDashboard");
                    }

                    return RedirectToAction("Dashboard", "Account");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
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
                    // In production, you'd send a reset email
                    return View("ResetPassword");
                }
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword()
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
