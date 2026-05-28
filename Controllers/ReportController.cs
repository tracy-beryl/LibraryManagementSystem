using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LibraryManagementSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly LibraryDbContext _context;

        public ReportController(LibraryDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string filter, DateTime? startDate, DateTime? endDate)
        {
            var books = _context.Books
                .Include(b => b.Loans)
                    .ThenInclude(l => l.User)
                .ToList();

            foreach (var book in books)
            {
                var activeLoans = book.Loans?.Count(l => l.ReturnDate == null) ?? 0;
                book.AvailableCopies = book.TotalCopies - activeLoans;
            }

            var allLoans = books
                .SelectMany(b => b.Loans.Select(l => new { Book = b, Loan = l }))
                .AsQueryable();

            if (startDate.HasValue)
            {
                allLoans = allLoans.Where(x => x.Loan.LoanDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                allLoans = allLoans.Where(x => x.Loan.LoanDate.Date <= endDate.Value.Date);
            }

            var model = new ReportPageViewModel
            {
                Filter = filter,
                StartDate = startDate,
                EndDate = endDate,
                TotalBooks = books.Count,
                OverdueCount = books.SelectMany(b => b.Loans).Count(l => l.DueDate < DateTime.Now && l.ReturnDate == null),
                OutOfStockCount = books.Count(b => b.AvailableCopies <= 0),
                TotalOutstandingFines = books.SelectMany(b => b.Loans)
                    .Sum(l => Math.Max(((l.FrozenFineAmount ?? 0) - l.FineAmountPaid), 0))
            };

            switch (filter)
            {
                case "NotReturned":
                    model.ReportTitle = "Books Not Returned Report";
                    model.Rows = allLoans
                        .Where(x => x.Loan.ReturnDate == null)
                        .Select(x => new ReportRowViewModel
                        {
                            BookId = x.Book.Id,
                            Title = x.Book.Title,
                            Author = x.Book.Author,
                            Category = x.Book.Category,
                            ReferenceNumber = x.Book.ReferenceNumber,
                            TotalCopies = x.Book.TotalCopies,
                            AvailableCopies = x.Book.AvailableCopies,
                            TimesBorrowed = x.Book.Loans.Count,
                            BorrowerName = x.Loan.User != null ? x.Loan.User.UserName : "Unknown User",
                            LoanDate = x.Loan.LoanDate,
                            DueDate = x.Loan.DueDate,
                            ReturnDate = x.Loan.ReturnDate,
                            Status = x.Loan.Status.ToString(),
                            FrozenFineAmount = x.Loan.FrozenFineAmount,
                            FineAmountPaid = x.Loan.FineAmountPaid,
                            OutstandingFine = Math.Max((x.Loan.FrozenFineAmount ?? 0) - x.Loan.FineAmountPaid, 0),
                            FinePaymentStatus = x.Loan.FinePaymentStatus.ToString(),
                            MpesaPaymentPending = x.Loan.MpesaPaymentPending,
                            ReplacementCost = x.Loan.ReplacementCost
                        })
                        .ToList();
                    break;

                case "Overdue":
                    model.ReportTitle = "Overdue Books Report";
                    model.Rows = allLoans
                        .Where(x => x.Loan.DueDate < DateTime.Now && x.Loan.ReturnDate == null)
                        .Select(x => new ReportRowViewModel
                        {
                            BookId = x.Book.Id,
                            Title = x.Book.Title,
                            Author = x.Book.Author,
                            Category = x.Book.Category,
                            ReferenceNumber = x.Book.ReferenceNumber,
                            TotalCopies = x.Book.TotalCopies,
                            AvailableCopies = x.Book.AvailableCopies,
                            TimesBorrowed = x.Book.Loans.Count,
                            BorrowerName = x.Loan.User != null ? x.Loan.User.UserName : "Unknown User",
                            LoanDate = x.Loan.LoanDate,
                            DueDate = x.Loan.DueDate,
                            ReturnDate = x.Loan.ReturnDate,
                            Status = x.Loan.Status.ToString(),
                            FrozenFineAmount = x.Loan.FrozenFineAmount,
                            FineAmountPaid = x.Loan.FineAmountPaid,
                            OutstandingFine = Math.Max((x.Loan.FrozenFineAmount ?? 0) - x.Loan.FineAmountPaid, 0),
                            FinePaymentStatus = x.Loan.FinePaymentStatus.ToString(),
                            MpesaPaymentPending = x.Loan.MpesaPaymentPending,
                            ReplacementCost = x.Loan.ReplacementCost
                        })
                        .ToList();
                    break;

                case "MostBorrowed":
                    model.ReportTitle = "Most Borrowed Books Report";
                    model.Rows = books
                        .OrderByDescending(b => b.Loans.Count)
                        .Take(10)
                        .Select(b => new ReportRowViewModel
                        {
                            BookId = b.Id,
                            Title = b.Title,
                            Author = b.Author,
                            Category = b.Category,
                            ReferenceNumber = b.ReferenceNumber,
                            TotalCopies = b.TotalCopies,
                            AvailableCopies = b.AvailableCopies,
                            TimesBorrowed = b.Loans.Count
                        })
                        .ToList();
                    break;

                case "AvailableCopies":
                    model.ReportTitle = "Available Copies Report";
                    model.Rows = books
                        .Where(b => b.AvailableCopies > 0)
                        .Select(b => new ReportRowViewModel
                        {
                            BookId = b.Id,
                            Title = b.Title,
                            Author = b.Author,
                            Category = b.Category,
                            ReferenceNumber = b.ReferenceNumber,
                            TotalCopies = b.TotalCopies,
                            AvailableCopies = b.AvailableCopies,
                            TimesBorrowed = b.Loans.Count
                        })
                        .ToList();
                    break;

                case "OutOfStock":
                    model.ReportTitle = "Out of Stock Books Report";
                    model.Rows = books
                        .Where(b => b.AvailableCopies <= 0)
                        .Select(b => new ReportRowViewModel
                        {
                            BookId = b.Id,
                            Title = b.Title,
                            Author = b.Author,
                            Category = b.Category,
                            ReferenceNumber = b.ReferenceNumber,
                            TotalCopies = b.TotalCopies,
                            AvailableCopies = b.AvailableCopies,
                            TimesBorrowed = b.Loans.Count
                        })
                        .ToList();
                    break;

                case "NeverBorrowed":
                    model.ReportTitle = "Never Borrowed Books Report";
                    model.Rows = books
                        .Where(b => b.Loans == null || !b.Loans.Any())
                        .Select(b => new ReportRowViewModel
                        {
                            BookId = b.Id,
                            Title = b.Title,
                            Author = b.Author,
                            Category = b.Category,
                            ReferenceNumber = b.ReferenceNumber,
                            TotalCopies = b.TotalCopies,
                            AvailableCopies = b.AvailableCopies,
                            TimesBorrowed = 0
                        })
                        .ToList();
                    break;

                case "LowStock":
                    model.ReportTitle = "Low Stock Books Report";
                    model.Rows = books
                        .Where(b => b.AvailableCopies > 0 && b.AvailableCopies <= 2)
                        .Select(b => new ReportRowViewModel
                        {
                            BookId = b.Id,
                            Title = b.Title,
                            Author = b.Author,
                            Category = b.Category,
                            ReferenceNumber = b.ReferenceNumber,
                            TotalCopies = b.TotalCopies,
                            AvailableCopies = b.AvailableCopies,
                            TimesBorrowed = b.Loans.Count
                        })
                        .ToList();
                    break;

                case "ReturnedBooks":
                    model.ReportTitle = "Returned Books Report";
                    model.Rows = allLoans
                        .Where(x => x.Loan.ReturnDate != null)
                        .Select(x => new ReportRowViewModel
                        {
                            BookId = x.Book.Id,
                            Title = x.Book.Title,
                            Author = x.Book.Author,
                            Category = x.Book.Category,
                            ReferenceNumber = x.Book.ReferenceNumber,
                            TotalCopies = x.Book.TotalCopies,
                            AvailableCopies = x.Book.AvailableCopies,
                            TimesBorrowed = x.Book.Loans.Count,
                            BorrowerName = x.Loan.User != null ? x.Loan.User.UserName : "Unknown User",
                            LoanDate = x.Loan.LoanDate,
                            DueDate = x.Loan.DueDate,
                            ReturnDate = x.Loan.ReturnDate,
                            Status = x.Loan.Status.ToString(),
                            FrozenFineAmount = x.Loan.FrozenFineAmount,
                            FineAmountPaid = x.Loan.FineAmountPaid,
                            OutstandingFine = Math.Max((x.Loan.FrozenFineAmount ?? 0) - x.Loan.FineAmountPaid, 0),
                            FinePaymentStatus = x.Loan.FinePaymentStatus.ToString(),
                            MpesaPaymentPending = x.Loan.MpesaPaymentPending,
                            ReplacementCost = x.Loan.ReplacementCost
                        })
                        .ToList();
                    break;

                case "UnpaidFines":
                    model.ReportTitle = "Unpaid Fines Report";
                    model.Rows = allLoans
                        .Where(x => (x.Loan.FrozenFineAmount ?? 0) > 0 && x.Loan.FineAmountPaid <= 0)
                        .Select(x => new ReportRowViewModel
                        {
                            BookId = x.Book.Id,
                            Title = x.Book.Title,
                            Author = x.Book.Author,
                            Category = x.Book.Category,
                            ReferenceNumber = x.Book.ReferenceNumber,
                            BorrowerName = x.Loan.User != null ? x.Loan.User.UserName : "Unknown User",
                            LoanDate = x.Loan.LoanDate,
                            DueDate = x.Loan.DueDate,
                            ReturnDate = x.Loan.ReturnDate,
                            Status = x.Loan.Status.ToString(),
                            FrozenFineAmount = x.Loan.FrozenFineAmount,
                            FineAmountPaid = x.Loan.FineAmountPaid,
                            OutstandingFine = Math.Max((x.Loan.FrozenFineAmount ?? 0) - x.Loan.FineAmountPaid, 0),
                            FinePaymentStatus = x.Loan.FinePaymentStatus.ToString(),
                            MpesaPaymentPending = x.Loan.MpesaPaymentPending,
                            ReplacementCost = x.Loan.ReplacementCost
                        })
                        .ToList();
                    break;

                case "PartiallyPaidFines":
                    model.ReportTitle = "Partially Paid Fines Report";
                    model.Rows = allLoans
                        .Where(x => (x.Loan.FrozenFineAmount ?? 0) > x.Loan.FineAmountPaid && x.Loan.FineAmountPaid > 0)
                        .Select(x => new ReportRowViewModel
                        {
                            BookId = x.Book.Id,
                            Title = x.Book.Title,
                            Author = x.Book.Author,
                            Category = x.Book.Category,
                            ReferenceNumber = x.Book.ReferenceNumber,
                            BorrowerName = x.Loan.User != null ? x.Loan.User.UserName : "Unknown User",
                            LoanDate = x.Loan.LoanDate,
                            DueDate = x.Loan.DueDate,
                            ReturnDate = x.Loan.ReturnDate,
                            Status = x.Loan.Status.ToString(),
                            FrozenFineAmount = x.Loan.FrozenFineAmount,
                            FineAmountPaid = x.Loan.FineAmountPaid,
                            OutstandingFine = Math.Max((x.Loan.FrozenFineAmount ?? 0) - x.Loan.FineAmountPaid, 0),
                            FinePaymentStatus = x.Loan.FinePaymentStatus.ToString(),
                            MpesaPaymentPending = x.Loan.MpesaPaymentPending,
                            ReplacementCost = x.Loan.ReplacementCost
                        })
                        .ToList();
                    break;

                case "FullyPaidFines":
                    model.ReportTitle = "Fully Paid Fines Report";
                    model.Rows = allLoans
                        .Where(x => (x.Loan.FrozenFineAmount ?? 0) > 0 &&
                                    x.Loan.FineAmountPaid >= (x.Loan.FrozenFineAmount ?? 0))
                        .Select(x => new ReportRowViewModel
                        {
                            BookId = x.Book.Id,
                            Title = x.Book.Title,
                            Author = x.Book.Author,
                            Category = x.Book.Category,
                            ReferenceNumber = x.Book.ReferenceNumber,
                            BorrowerName = x.Loan.User != null ? x.Loan.User.UserName : "Unknown User",
                            LoanDate = x.Loan.LoanDate,
                            DueDate = x.Loan.DueDate,
                            ReturnDate = x.Loan.ReturnDate,
                            Status = x.Loan.Status.ToString(),
                            FrozenFineAmount = x.Loan.FrozenFineAmount,
                            FineAmountPaid = x.Loan.FineAmountPaid,
                            OutstandingFine = Math.Max((x.Loan.FrozenFineAmount ?? 0) - x.Loan.FineAmountPaid, 0),
                            FinePaymentStatus = x.Loan.FinePaymentStatus.ToString(),
                            MpesaPaymentPending = x.Loan.MpesaPaymentPending,
                            ReplacementCost = x.Loan.ReplacementCost
                        })
                        .ToList();
                    break;

                case "PendingMpesa":
                    model.ReportTitle = "Pending M-Pesa Payments Report";
                    model.Rows = allLoans
                        .Where(x => x.Loan.MpesaPaymentPending)
                        .Select(x => new ReportRowViewModel
                        {
                            BookId = x.Book.Id,
                            Title = x.Book.Title,
                            Author = x.Book.Author,
                            Category = x.Book.Category,
                            ReferenceNumber = x.Book.ReferenceNumber,
                            BorrowerName = x.Loan.User != null ? x.Loan.User.UserName : "Unknown User",
                            LoanDate = x.Loan.LoanDate,
                            DueDate = x.Loan.DueDate,
                            ReturnDate = x.Loan.ReturnDate,
                            Status = x.Loan.Status.ToString(),
                            FrozenFineAmount = x.Loan.FrozenFineAmount,
                            FineAmountPaid = x.Loan.FineAmountPaid,
                            OutstandingFine = Math.Max((x.Loan.FrozenFineAmount ?? 0) - x.Loan.FineAmountPaid, 0),
                            FinePaymentStatus = x.Loan.FinePaymentStatus.ToString(),
                            MpesaPaymentPending = x.Loan.MpesaPaymentPending,
                            ReplacementCost = x.Loan.ReplacementCost
                        })
                        .ToList();
                    break;

                default:
                    model.ReportTitle = "All Library Book Reports";
                    model.Rows = books
                        .Select(b => new ReportRowViewModel
                        {
                            BookId = b.Id,
                            Title = b.Title,
                            Author = b.Author,
                            Category = b.Category,
                            ReferenceNumber = b.ReferenceNumber,
                            TotalCopies = b.TotalCopies,
                            AvailableCopies = b.AvailableCopies,
                            TimesBorrowed = b.Loans?.Count ?? 0
                        })
                        .ToList();
                    break;
            }

            model.TotalRows = model.Rows.Count;

            return View(model);
        }
    }
}