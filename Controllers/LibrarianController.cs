using ClosedXML.Excel;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Librarian")]
    public class LibrarianController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LibrarianController(
            LibraryDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private void SyncBookCopiesFromInventory(InventoryRecord inventory)
        {
            if (inventory?.Book == null)
                return;

            inventory.Book.TotalCopies = inventory.TotalCopies;
            inventory.Book.AvailableCopies = inventory.AvailableCopies;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var firstName = user?.FirstName ?? "Librarian";

            var today = DateTime.Today;
            var now = DateTime.Now;

            // Loans data
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .ToListAsync();

            var activeLoans = loans
                .Where(l => l.ReturnDate == null && l.Status == LoanStatus.Borrowed)
                .ToList();

            var issuedToday = loans.Count(l => l.LoanDate.Date == today);
            var returnedToday = loans.Count(l => l.ReturnDate.HasValue && l.ReturnDate.Value.Date == today);

            var overdueLoans = activeLoans
                .Where(l => l.DueDate < now)
                .OrderBy(l => l.DueDate)
                .ToList();

            // Top borrowed books
            var topBorrowedBooks = await _context.Books
                .Include(b => b.Loans)
                .OrderByDescending(b => b.Loans.Count())
                .Take(5)
                .ToListAsync();

            // Recent returns
            var recentReturns = loans
                .Where(l => l.ReturnDate.HasValue)
                .OrderByDescending(l => l.ReturnDate)
                .Take(10)
                .ToList();

            // Fines from overdue loans
            var unpaidFinesCount = overdueLoans.Count(l => (l.FrozenFineAmount ?? 0) > 0);
            var unpaidFinesAmount = overdueLoans.Sum(l => l.FrozenFineAmount ?? 0);

            // Inventory items
            var lowStockItems = await _context.InventoryRecords
                .Include(i => i.Book)
                .Where(i => i.AvailableCopies <= i.ReorderThreshold)
                .OrderBy(i => i.AvailableCopies)
                .Take(10)
                .ToListAsync();

            var damagedItems = await _context.InventoryRecords
                .Include(i => i.Book)
                .Where(i => i.DamagedCopies > 0)
                .OrderByDescending(i => i.DamagedCopies)
                .Take(10)
                .ToListAsync();

            var missingItems = await _context.InventoryRecords
                .Include(i => i.Book)
                .Where(i => i.MissingCopies > 0)
                .OrderByDescending(i => i.MissingCopies)
                .Take(10)
                .ToListAsync();

            // Damage reports
            var openDamageReports = await _context.DamageReports
                .Include(r => r.Book)
                .Where(r => r.Status == "Open")
                .OrderByDescending(r => r.ReportedAt)
                .Take(10)
                .ToListAsync();

            // Recent inventory transactions
            var recentInventoryTransactions = await _context.InventoryTransactions
                .Include(t => t.InventoryRecord)
                .ThenInclude(i => i.Book)
                .OrderByDescending(t => t.CreatedAt)
                .Take(10)
                .ToListAsync();

            var vm = new LibrarianDashboardViewModel
            {
                FirstName = firstName,
                BooksIssuedToday = issuedToday,
                BooksReturnedToday = returnedToday,
                ActiveLoansCount = activeLoans.Count,
                OverdueLoansCount = overdueLoans.Count,
                UnpaidFinesCount = unpaidFinesCount,
                UnpaidFinesAmount = unpaidFinesAmount,
                LowStockItemsCount = lowStockItems.Count,
                DamagedItemsCount = damagedItems.Count,
                MissingItemsCount = missingItems.Count,
                OpenDamageReportsCount = openDamageReports.Count,
                OverdueLoans = overdueLoans.Take(10).ToList(),
                RecentReturns = recentReturns,
                TopBorrowedBooks = topBorrowedBooks,
                LowStockItems = lowStockItems,
                DamagedItems = damagedItems,
                MissingItems = missingItems,
                OpenDamageReports = openDamageReports,
                RecentInventoryTransactions = recentInventoryTransactions
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> InitializeInventory()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var books = await _context.Books
                .Include(b => b.Loans)
                .Include(b => b.InventoryRecord)
                .ToListAsync();

            foreach (var book in books)
            {
                var borrowedCopies = book.Loans.Count(l => l.ReturnDate == null && l.Status == LoanStatus.Borrowed);
                var availableCopies = book.TotalCopies - borrowedCopies;
                if (availableCopies < 0)
                    availableCopies = 0;

                // keep Book table aligned too
                book.AvailableCopies = availableCopies;

                if (book.InventoryRecord == null)
                {
                    var record = new InventoryRecord
                    {
                        BookId = book.Id,
                        TotalCopies = book.TotalCopies,
                        AvailableCopies = availableCopies,
                        DamagedCopies = 0,
                        MissingCopies = 0,
                        ReorderThreshold = 2,
                        LastUpdatedAt = DateTime.Now,
                        LastUpdatedByUserId = currentUser?.Id
                    };

                    _context.InventoryRecords.Add(record);
                }
                else
                {
                    // optional re-sync existing inventory records
                    book.InventoryRecord.TotalCopies = book.TotalCopies;
                    book.InventoryRecord.AvailableCopies = availableCopies;
                    book.InventoryRecord.LastUpdatedAt = DateTime.Now;
                    book.InventoryRecord.LastUpdatedByUserId = currentUser?.Id;
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Inventory records initialized successfully.";
            return RedirectToAction("Dashboard", "Librarian");
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> ReportDamage(int? bookId)
        {
            ViewBag.Books = await _context.Books
                .OrderBy(b => b.Title)
                .ToListAsync();

            var model = new ReportDamageViewModel();

            if (bookId.HasValue)
                model.BookId = bookId.Value;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> ReportDamage(ReportDamageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Books = await _context.Books
                    .OrderBy(b => b.Title)
                    .ToListAsync();

                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var inventory = await _context.InventoryRecords
                .FirstOrDefaultAsync(i => i.BookId == model.BookId);

            if (inventory == null)
            {
                TempData["Error"] = "No inventory record found for the selected book.";
                return RedirectToAction("Dashboard");
            }

            if (model.QuantityAffected > inventory.AvailableCopies)
            {
                ModelState.AddModelError("QuantityAffected", "Quantity affected cannot exceed available copies.");

                ViewBag.Books = await _context.Books
                    .OrderBy(b => b.Title)
                    .ToListAsync();

                return View(model);
            }

            var previousAvailable = inventory.AvailableCopies;

            inventory.AvailableCopies -= model.QuantityAffected;

            if (model.ReportType == "Damaged")
                inventory.DamagedCopies += model.QuantityAffected;
            else if (model.ReportType == "Missing")
                inventory.MissingCopies += model.QuantityAffected;

            SyncBookCopiesFromInventory(inventory);

            inventory.LastUpdatedAt = DateTime.Now;
            inventory.LastUpdatedByUserId = currentUser?.Id;

            var report = new DamageReport
            {
                BookId = model.BookId,
                ReportType = model.ReportType,
                Description = model.Description,
                QuantityAffected = model.QuantityAffected,
                Status = "Open",
                ReportedAt = DateTime.Now,
                ReportedByUserId = currentUser?.Id
            };

            _context.DamageReports.Add(report);

            var transaction = new InventoryTransaction
            {
                InventoryRecordId = inventory.Id,
                TransactionType = model.ReportType == "Damaged"
                    ? InventoryTransactionType.Damaged
                    : InventoryTransactionType.Missing,
                QuantityChanged = model.QuantityAffected,
                PreviousAvailableCopies = previousAvailable,
                NewAvailableCopies = inventory.AvailableCopies,
                Notes = model.Description,
                CreatedAt = DateTime.Now,
                PerformedByUserId = currentUser?.Id
            };

            _context.InventoryTransactions.Add(transaction);

            await _context.SaveChangesAsync();

            TempData["Success"] = $"{model.ReportType} report recorded successfully.";
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> AddStock(int? bookId)
        {
            ViewBag.Books = await _context.Books
                .OrderBy(b => b.Title)
                .ToListAsync();

            var model = new AddStockViewModel();

            if (bookId.HasValue)
                model.BookId = bookId.Value;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> AddStock(AddStockViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Books = await _context.Books
                    .OrderBy(b => b.Title)
                    .ToListAsync();

                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var inventory = await _context.InventoryRecords
                .Include(i => i.Book)
                .FirstOrDefaultAsync(i => i.BookId == model.BookId);

            if (inventory == null)
            {
                ModelState.AddModelError("", "No inventory record found for the selected book.");

                ViewBag.Books = await _context.Books
                    .OrderBy(b => b.Title)
                    .ToListAsync();

                return View(model);
            }

            var previousAvailable = inventory.AvailableCopies;

            inventory.TotalCopies += model.QuantityAdded;
            inventory.AvailableCopies += model.QuantityAdded;

            SyncBookCopiesFromInventory(inventory);

            inventory.LastUpdatedAt = DateTime.Now;
            inventory.LastUpdatedByUserId = currentUser?.Id;

            var transaction = new InventoryTransaction
            {
                InventoryRecordId = inventory.Id,
                TransactionType = InventoryTransactionType.Added,
                QuantityChanged = model.QuantityAdded,
                PreviousAvailableCopies = previousAvailable,
                NewAvailableCopies = inventory.AvailableCopies,
                Notes = string.IsNullOrWhiteSpace(model.Notes)
                    ? $"Added {model.QuantityAdded} new copie(s)."
                    : model.Notes.Trim(),
                CreatedAt = DateTime.Now,
                PerformedByUserId = currentUser?.Id
            };

            _context.InventoryTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Stock added successfully for '{inventory.Book?.Title}'.";
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> ResolveDamageReport(int id)
        {
            var report = await _context.DamageReports
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
                return NotFound();

            var vm = new ResolveDamageReportViewModel
            {
                ReportId = report.Id,
                BookId = report.BookId,
                BookTitle = report.Book?.Title,
                ReportType = report.ReportType,
                QuantityAffected = report.QuantityAffected,
                Description = report.Description
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> ResolveDamageReport(ResolveDamageReportViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var currentUser = await _userManager.GetUserAsync(User);

            var report = await _context.DamageReports
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == model.ReportId);

            if (report == null)
                return NotFound();

            if (report.Status != "Open")
            {
                TempData["Error"] = "This report has already been resolved.";
                return RedirectToAction("Dashboard");
            }

            var inventory = await _context.InventoryRecords
                .Include(i => i.Book)
                .FirstOrDefaultAsync(i => i.BookId == report.BookId);

            if (inventory == null)
            {
                TempData["Error"] = "No inventory record found for the selected report.";
                return RedirectToAction("Dashboard");
            }

            var previousAvailable = inventory.AvailableCopies;
            InventoryTransactionType transactionType;

            if (report.ReportType == "Damaged")
            {
                if (model.ResolutionAction == "Repaired")
                {
                    if (inventory.DamagedCopies < report.QuantityAffected)
                    {
                        TempData["Error"] = "Damaged copy count is lower than the report quantity.";
                        return RedirectToAction("Dashboard");
                    }

                    inventory.DamagedCopies -= report.QuantityAffected;
                    inventory.AvailableCopies += report.QuantityAffected;
                    transactionType = InventoryTransactionType.Repaired;
                }
                else if (model.ResolutionAction == "WrittenOff")
                {
                    if (inventory.DamagedCopies < report.QuantityAffected)
                    {
                        TempData["Error"] = "Damaged copy count is lower than the report quantity.";
                        return RedirectToAction("Dashboard");
                    }

                    inventory.DamagedCopies -= report.QuantityAffected;
                    inventory.TotalCopies -= report.QuantityAffected;
                    if (inventory.TotalCopies < 0) inventory.TotalCopies = 0;

                    transactionType = InventoryTransactionType.Adjusted;
                }
                else
                {
                    ModelState.AddModelError("ResolutionAction", "Invalid resolution action for a damaged report.");
                    return View(model);
                }
            }
            else if (report.ReportType == "Missing")
            {
                if (model.ResolutionAction == "Recovered")
                {
                    if (inventory.MissingCopies < report.QuantityAffected)
                    {
                        TempData["Error"] = "Missing copy count is lower than the report quantity.";
                        return RedirectToAction("Dashboard");
                    }

                    inventory.MissingCopies -= report.QuantityAffected;
                    inventory.AvailableCopies += report.QuantityAffected;
                    transactionType = InventoryTransactionType.Recovered;
                }
                else if (model.ResolutionAction == "WrittenOff")
                {
                    if (inventory.MissingCopies < report.QuantityAffected)
                    {
                        TempData["Error"] = "Missing copy count is lower than the report quantity.";
                        return RedirectToAction("Dashboard");
                    }

                    inventory.MissingCopies -= report.QuantityAffected;
                    inventory.TotalCopies -= report.QuantityAffected;
                    if (inventory.TotalCopies < 0) inventory.TotalCopies = 0;

                    transactionType = InventoryTransactionType.Adjusted;
                }
                else
                {
                    ModelState.AddModelError("ResolutionAction", "Invalid resolution action for a missing report.");
                    return View(model);
                }
            }
            else
            {
                TempData["Error"] = "Unknown report type.";
                return RedirectToAction("Dashboard");
            }

            SyncBookCopiesFromInventory(inventory);

            inventory.LastUpdatedAt = DateTime.Now;
            inventory.LastUpdatedByUserId = currentUser?.Id;

            report.Status = "Resolved";

            var notes = string.IsNullOrWhiteSpace(model.ResolutionNotes)
                ? $"{model.ResolutionAction} applied to report #{report.Id}."
                : model.ResolutionNotes.Trim();

            var transaction = new InventoryTransaction
            {
                InventoryRecordId = inventory.Id,
                TransactionType = transactionType,
                QuantityChanged = report.QuantityAffected,
                PreviousAvailableCopies = previousAvailable,
                NewAvailableCopies = inventory.AvailableCopies,
                Notes = notes,
                CreatedAt = DateTime.Now,
                PerformedByUserId = currentUser?.Id
            };

            _context.InventoryTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Damage report resolved successfully.";
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Inventory(string searchTerm, string statusFilter)
        {
            var query = _context.InventoryRecords
                .Include(i => i.Book)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();

                query = query.Where(i =>
                    i.Book.Title.ToLower().Contains(term) ||
                    (i.Book.Author != null && i.Book.Author.ToLower().Contains(term)) ||
                    (i.Book.Category != null && i.Book.Category.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                switch (statusFilter)
                {
                    case "LowStock":
                        query = query.Where(i => i.AvailableCopies <= i.ReorderThreshold);
                        break;

                    case "Damaged":
                        query = query.Where(i => i.DamagedCopies > 0);
                        break;

                    case "Missing":
                        query = query.Where(i => i.MissingCopies > 0);
                        break;

                    case "Healthy":
                        query = query.Where(i =>
                            i.AvailableCopies > i.ReorderThreshold &&
                            i.DamagedCopies == 0 &&
                            i.MissingCopies == 0);
                        break;
                }
            }

            var items = await query
                .OrderBy(i => i.Book.Title)
                .Select(i => new InventoryListItemViewModel
                {
                    InventoryRecordId = i.Id,
                    BookId = i.BookId,
                    BookTitle = i.Book.Title,
                    Author = i.Book.Author,
                    Category = i.Book.Category,
                    TotalCopies = i.TotalCopies,
                    AvailableCopies = i.AvailableCopies,
                    DamagedCopies = i.DamagedCopies,
                    MissingCopies = i.MissingCopies,
                    ReorderThreshold = i.ReorderThreshold,
                    LastUpdatedAt = i.LastUpdatedAt
                })
                .ToListAsync();

            var vm = new InventoryIndexViewModel
            {
                SearchTerm = searchTerm,
                StatusFilter = statusFilter,
                Items = items
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> InventoryHistory(int bookId)
        {
            var inventory = await _context.InventoryRecords
                .Include(i => i.Book)
                .FirstOrDefaultAsync(i => i.BookId == bookId);

            if (inventory == null)
            {
                TempData["Error"] = "No inventory record found for the selected book.";
                return RedirectToAction("Inventory");
            }

            var transactions = await _context.InventoryTransactions
                .Include(t => t.PerformedByUser)
                .Where(t => t.InventoryRecordId == inventory.Id)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new InventoryHistoryItemViewModel
                {
                    TransactionType = t.TransactionType.ToString(),
                    QuantityChanged = t.QuantityChanged,
                    PreviousAvailableCopies = t.PreviousAvailableCopies,
                    NewAvailableCopies = t.NewAvailableCopies,
                    Notes = t.Notes,
                    CreatedAt = t.CreatedAt,
                    PerformedByName = t.PerformedByUser != null
                        ? (!string.IsNullOrWhiteSpace(t.PerformedByUser.FullName)
                            ? t.PerformedByUser.FullName
                            : t.PerformedByUser.Email)
                        : "Unknown"
                })
                .ToListAsync();

            var vm = new InventoryHistoryViewModel
            {
                BookId = inventory.BookId,
                BookTitle = inventory.Book?.Title,
                Author = inventory.Book?.Author,
                Category = inventory.Book?.Category,
                TotalCopies = inventory.TotalCopies,
                AvailableCopies = inventory.AvailableCopies,
                DamagedCopies = inventory.DamagedCopies,
                MissingCopies = inventory.MissingCopies,
                ReorderThreshold = inventory.ReorderThreshold,
                Transactions = transactions
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> EditInventory(int id)
        {
            var inventory = await _context.InventoryRecords
                .Include(i => i.Book)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
            {
                TempData["Error"] = "Inventory record not found.";
                return RedirectToAction("Inventory");
            }

            var vm = new EditInventoryViewModel
            {
                InventoryRecordId = inventory.Id,
                BookId = inventory.BookId,
                BookTitle = inventory.Book?.Title,
                TotalCopies = inventory.TotalCopies,
                AvailableCopies = inventory.AvailableCopies,
                DamagedCopies = inventory.DamagedCopies,
                MissingCopies = inventory.MissingCopies,
                ReorderThreshold = inventory.ReorderThreshold
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> EditInventory(EditInventoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var currentUser = await _userManager.GetUserAsync(User);

            var inventory = await _context.InventoryRecords
                .Include(i => i.Book)
                .FirstOrDefaultAsync(i => i.Id == model.InventoryRecordId);

            if (inventory == null)
            {
                TempData["Error"] = "Inventory record not found.";
                return RedirectToAction("Inventory");
            }

            var oldThreshold = inventory.ReorderThreshold;

            inventory.ReorderThreshold = model.ReorderThreshold;
            inventory.LastUpdatedAt = DateTime.Now;
            inventory.LastUpdatedByUserId = currentUser?.Id;

            if (oldThreshold != model.ReorderThreshold)
            {
                var transaction = new InventoryTransaction
                {
                    InventoryRecordId = inventory.Id,
                    TransactionType = InventoryTransactionType.Adjusted,
                    QuantityChanged = 0,
                    PreviousAvailableCopies = inventory.AvailableCopies,
                    NewAvailableCopies = inventory.AvailableCopies,
                    Notes = string.IsNullOrWhiteSpace(model.Notes)
                        ? $"Reorder threshold changed from {oldThreshold} to {model.ReorderThreshold}."
                        : model.Notes.Trim(),
                    CreatedAt = DateTime.Now,
                    PerformedByUserId = currentUser?.Id
                };

                _context.InventoryTransactions.Add(transaction);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Inventory settings updated for '{inventory.Book?.Title}'.";
            return RedirectToAction("Inventory");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> ExportInventoryExcel(string searchTerm, string statusFilter)
        {
            var query = _context.InventoryRecords
                .Include(i => i.Book)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();

                query = query.Where(i =>
                    i.Book.Title.ToLower().Contains(term) ||
                    (i.Book.Author != null && i.Book.Author.ToLower().Contains(term)) ||
                    (i.Book.Category != null && i.Book.Category.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                switch (statusFilter)
                {
                    case "LowStock":
                        query = query.Where(i => i.AvailableCopies <= i.ReorderThreshold);
                        break;

                    case "Damaged":
                        query = query.Where(i => i.DamagedCopies > 0);
                        break;

                    case "Missing":
                        query = query.Where(i => i.MissingCopies > 0);
                        break;

                    case "Healthy":
                        query = query.Where(i =>
                            i.AvailableCopies > i.ReorderThreshold &&
                            i.DamagedCopies == 0 &&
                            i.MissingCopies == 0);
                        break;
                }
            }

            var items = await query
                .OrderBy(i => i.Book.Title)
                .Select(i => new
                {
                    Title = i.Book.Title,
                    Author = i.Book.Author,
                    Category = i.Book.Category,
                    TotalCopies = i.TotalCopies,
                    AvailableCopies = i.AvailableCopies,
                    DamagedCopies = i.DamagedCopies,
                    MissingCopies = i.MissingCopies,
                    ReorderThreshold = i.ReorderThreshold,
                    Status =
                        i.MissingCopies > 0 ? "Missing" :
                        i.DamagedCopies > 0 ? "Damaged" :
                        i.AvailableCopies <= i.ReorderThreshold ? "Low Stock" :
                        "Healthy",
                    LastUpdatedAt = i.LastUpdatedAt
                })
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Inventory");

            // Title
            worksheet.Cell(1, 1).Value = "Library Inventory Report";
            worksheet.Range(1, 1, 1, 10).Merge().Style.Font.Bold = true;
            worksheet.Range(1, 1, 1, 10).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Filters summary
            worksheet.Cell(2, 1).Value = "Search Term:";
            worksheet.Cell(2, 2).Value = searchTerm ?? "All";

            worksheet.Cell(3, 1).Value = "Status Filter:";
            worksheet.Cell(3, 2).Value = statusFilter ?? "All";

            worksheet.Cell(4, 1).Value = "Generated On:";
            worksheet.Cell(4, 2).Value = DateTime.Now.ToString("dd MMM yyyy HH:mm");

            // Headers
            var row = 6;
            worksheet.Cell(row, 1).Value = "Title";
            worksheet.Cell(row, 2).Value = "Author";
            worksheet.Cell(row, 3).Value = "Category";
            worksheet.Cell(row, 4).Value = "Total Copies";
            worksheet.Cell(row, 5).Value = "Available Copies";
            worksheet.Cell(row, 6).Value = "Damaged Copies";
            worksheet.Cell(row, 7).Value = "Missing Copies";
            worksheet.Cell(row, 8).Value = "Reorder Threshold";
            worksheet.Cell(row, 9).Value = "Status";
            worksheet.Cell(row, 10).Value = "Last Updated";

            var headerRange = worksheet.Range(row, 1, row, 10);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.DarkBlue;
            headerRange.Style.Font.FontColor = XLColor.White;

            // Data
            row++;

            foreach (var item in items)
            {
                worksheet.Cell(row, 1).Value = item.Title;
                worksheet.Cell(row, 2).Value = item.Author;
                worksheet.Cell(row, 3).Value = item.Category;
                worksheet.Cell(row, 4).Value = item.TotalCopies;
                worksheet.Cell(row, 5).Value = item.AvailableCopies;
                worksheet.Cell(row, 6).Value = item.DamagedCopies;
                worksheet.Cell(row, 7).Value = item.MissingCopies;
                worksheet.Cell(row, 8).Value = item.ReorderThreshold;
                worksheet.Cell(row, 9).Value = item.Status;
                worksheet.Cell(row, 10).Value = item.LastUpdatedAt.ToString("dd MMM yyyy");

                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"InventoryReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

     
    }
}