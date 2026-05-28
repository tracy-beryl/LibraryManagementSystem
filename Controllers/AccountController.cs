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
using LibraryManagementSystem.Service;

namespace LibraryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly LibraryDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            LibraryDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


        // ADMIN DASHBOARD

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var firstName = !string.IsNullOrWhiteSpace(currentUser?.FirstName)
      ? currentUser.FirstName
      : currentUser?.FullName?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "Admin";
            var today = DateTime.Today;

            var totalBooks = await _context.Books.CountAsync();

            var activeLoans = await _context.Loans
                .Where(l => l.ReturnDate == null)
                .ToListAsync();

            var overdueBooks = activeLoans.Count(l => l.DueDate < DateTime.Now);
            var pastPapers = await _context.PastPapers.CountAsync();
            var users = await _userManager.Users.ToListAsync();

            var books = await _context.Books
                .Include(b => b.Loans)
                .Include(b => b.InventoryRecord)
                .ToListAsync();

            foreach (var book in books)
            {
                if (book.InventoryRecord != null)
                {
                    book.TotalCopies = book.InventoryRecord.TotalCopies;
                    book.AvailableCopies = book.InventoryRecord.AvailableCopies;
                }
            }

            var availableBooks = books.Sum(b => b.AvailableCopies);
            var borrowedBooks = activeLoans.Count;

            var totalVisitorsToday = users.Count(u =>
                u.LastLoginDate.HasValue &&
                u.LastLoginDate.Value.Date == today);

            var newMembers = users.Count(u =>
                u.CreatedAt >= DateTime.Now.AddDays(-3));

            var topBooks = books
                .OrderByDescending(b => b.Loans.Count)
                .Take(5)
                .ToList();

            var visitorStats = Enumerable.Range(0, 7)
                .Select(i =>
                {
                    var date = today.AddDays(-i);
                    var count = users.Count(u =>
                        u.LastLoginDate.HasValue &&
                        u.LastLoginDate.Value.Date == date);

                    return new VisitorStat
                    {
                        Date = date,
                        Count = count
                    };
                })
                .Reverse()
                .ToList();

            var categoryData = books
                .GroupBy(b => string.IsNullOrEmpty(b.Category)
                    ? "Uncategorized"
                    : b.Category)
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

        // =========================
        // REGISTER STUDENT
        [Authorize(Roles = "Student")]
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
            if (!ModelState.IsValid)
                return View(model);

            // Enforce two or more names
            if (string.IsNullOrWhiteSpace(model.FullName) ||
                model.FullName.Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Length < 2)
            {
                ModelState.AddModelError("FullName",
                    "Please enter at least two names.");
                return View(model);
            }

            if (!await _roleManager.RoleExistsAsync("Student"))
            {
                ModelState.AddModelError("", "Student role is not configured.");
                return View(model);
            }

            var fullName = model.FullName.Trim();
            var nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = fullName,
                FirstName = nameParts[0],
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

            await _userManager.AddToRoleAsync(user, "Student");

            // Create StudentProfile
            var studentProfile = new StudentProfile
            {
                UserId = user.Id,
                AdmissionNumber = model.AdmissionNumber
            };

            _context.StudentProfiles.Add(studentProfile);
            await _context.SaveChangesAsync();

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("Index", "StudentDashboard");
        }

        // LOGIN
       
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
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.StaffProfile)
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            user.LastLoginDate = DateTime.Now;
            await _userManager.UpdateAsync(user);

            if (await _userManager.IsInRoleAsync(user, "Student"))
                return RedirectToAction("Index", "StudentDashboard");

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return RedirectToAction("Dashboard", "Account");

            if (await _userManager.IsInRoleAsync(user, "Librarian"))
                return RedirectToAction("Dashboard", "Librarian");

            if (await _userManager.IsInRoleAsync(user, "Lecturer"))
                return RedirectToAction("Dashboard", "Lecturer");

            return RedirectToAction("Index", "Home");
        }
        // LOGOUT

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        
        // FORGOT PASSWORD
       
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordViewModel model,
            [FromServices] IEmailSender emailSender)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                return View("ForgotPasswordConfirmation");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);

            var resetLink = Url.Action(
                "ResetPassword",
                "Account",
                new { token = encodedToken, email = user.Email },
                Request.Scheme);

            await emailSender.SendEmailAsync(
                user.Email,
                "Reset Password",
                $"Click <a href='{resetLink}'>here</a> to reset your password.");

            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
                return RedirectToAction("Login");

            return View(new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation");

            var decodedToken = System.Web.HttpUtility.UrlDecode(model.Token);

            var result = await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                model.Password);

            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }
    }
}