
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
    {
        public class FineController : Controller
        {
            private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FineController(LibraryDbContext context, UserManager<ApplicationUser> userManager)
            {
                _context = context;
            _userManager = userManager;
        }

            public IActionResult Index()
            {
                var today = DateTime.Now;

                var fines = _context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.User)
                    .Where(l => l.ReturnDate == null && (l.DueDate < today || l.DueDate < l.ReturnDate))
                    .Select(l => new FineViewModel
                    {
                        StudentName = l.User.FullName,
                        BookTitle = l.Book.Title,
                        DueDate = l.DueDate,
                        ReturnDate = l.ReturnDate,
                        DaysLate = EF.Functions.DateDiffDay(l.DueDate, today),
                        FineAmount = EF.Functions.DateDiffDay(l.DueDate, today) * 10
                    })
                    .ToList();

                return View(fines);
            }


       
        public async Task<IActionResult> MyFines()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var today = DateTime.Now;

            var fines = _context.Loans
                .Include(l => l.Book)
                .Where(l => l.UserId == user.Id && l.ReturnDate == null && l.DueDate < today)
                .Select(l => new FineViewModel
                {
                    StudentName = user.FullName,
                    BookTitle = l.Book.Title,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,
                    DaysLate = EF.Functions.DateDiffDay(l.DueDate, today),
                    FineAmount = EF.Functions.DateDiffDay(l.DueDate, today) * 10
                })
                .ToList();

            return View(fines);
        }
    }
    }



