using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    public class StudentDashboardController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentDashboardController(LibraryDbContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()

        {
            var currentUser = await _userManager.GetUserAsync(User);
            var firstName = currentUser?.FirstName ?? "User";

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var users = _context.Users.ToList();
            var myBorrowedBooks = _context.Loans.Count(l => l.UserId == user.Id && l.ReturnDate == null);
            var myReturnedBooks = _context.Loans.Count(l => l.UserId == user.Id && l.ReturnDate != null);

            var viewModel = new StudentDashboardViewModel
            {
                MyBorrowedBooks = myBorrowedBooks,
                MyHistory = myReturnedBooks,
                FirstName= firstName
            };

            return View(viewModel);
        }


    }
}
