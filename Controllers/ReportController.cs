
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LibraryManagementSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly LibraryDbContext _context;

        public ReportController(LibraryDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string filter)
        {
            var books = _context.Books.Include(b => b.Loans).ToList();

            // ✅ Recalculate AvailableCopies dynamically
            foreach (var book in books)
            {
                var activeLoans = book.Loans.Where(l => l.ReturnDate == null).Count();
                book.AvailableCopies = book.TotalCopies - activeLoans;
            }

            switch (filter)
            {
                case "NotReturned":
                    books = books
                        .Where(b => b.Loans.Any(l => l.ReturnDate == null))
                        .ToList();
                    break;

                case "Overdue":
                    books = books
                        .Where(b => b.Loans.Any(l => l.DueDate < DateTime.Now && l.ReturnDate == null))
                        .ToList();
                    break;

                case "MostBorrowed":
                    books = books
                        .OrderByDescending(b => b.Loans.Count)
                        .Take(10)
                        .ToList();
                    break;

                case "AvailableCopies":
                    books = books
                        .Where(b => b.AvailableCopies > 0)
                        .ToList();
                    break;
            }

            ViewBag.Filter = filter;
            return View(books);
        }

    }
}
