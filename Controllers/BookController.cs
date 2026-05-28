
using ClosedXML.Excel;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Service;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace LibraryManagementSystem.Controllers
{
    
    public class BookController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISmsService _sms;

        public BookController(LibraryDbContext context, UserManager<ApplicationUser> userManager, ISmsService sms)
        {
            _context = context;
            _userManager = userManager;
            _sms = sms;
        }

        private async Task<string> SaveBookPhotoAsync(IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "books");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            return "/images/books/" + fileName;
        }
        private async Task<InventoryRecord> GetOrCreateInventoryRecordAsync(Book book, string userId = null)
        {
            var inventory = await _context.InventoryRecords
                .FirstOrDefaultAsync(i => i.BookId == book.Id);

            if (inventory == null)
            {
                inventory = new InventoryRecord
                {
                    BookId = book.Id,
                    TotalCopies = book.TotalCopies,
                    AvailableCopies = book.AvailableCopies,
                    DamagedCopies = 0,
                    MissingCopies = 0,
                    ReorderThreshold = 2,
                    LastUpdatedAt = DateTime.Now,
                    LastUpdatedByUserId = userId
                };

                _context.InventoryRecords.Add(inventory);
            }

            return inventory;
        }

        private void SyncInventoryFromBook(Book book, InventoryRecord inventory, string userId = null)
        {
            if (book == null || inventory == null)
                return;

            inventory.TotalCopies = book.TotalCopies;
            inventory.AvailableCopies = book.AvailableCopies;
            inventory.LastUpdatedAt = DateTime.Now;
            inventory.LastUpdatedByUserId = userId;
        }

        private void ApplyInventoryValuesToBook(Book book)
        {
            if (book?.InventoryRecord == null)
                return;

            book.TotalCopies = book.InventoryRecord.TotalCopies;
            book.AvailableCopies = book.InventoryRecord.AvailableCopies;
        }

        private async Task<bool> BookExistsAsync(CreateBookViewModel model, int? ignoreId = null)
        {
            var isbn = (model.ISBN ?? "").Trim().ToLower();
            var edition = (model.Edition ?? "").Trim().ToLower();
            var department = (model.Department ?? "").Trim().ToLower();

            return await _context.Books.AnyAsync(b =>
                (ignoreId == null || b.Id != ignoreId.Value) &&
                (b.ISBN ?? "").ToLower() == isbn &&
                (b.Edition ?? "").ToLower() == edition &&
                (b.Department ?? "").ToLower() == department
            );
        }
        public IActionResult Filter(string department, string category)
        {
            var books = _context.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(department))
            {
                books = books.Where(b => b.Department == department);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                books = books.Where(b => b.Category == category);
            }

            var model = new BookViewModel
            {
                AllBooks = books
                    .Include(b => b.Loans)
                    .ToList(),
                Users = _context.Users.ToList()
            };

            return View("Index", model);
        }


        public ActionResult Index()
        {
            var model = new BookViewModel();

            var allBooks = _context.Books
                .Include(b => b.Loans)
                .Include(b => b.InventoryRecord)
                .ToList();

            foreach (var book in allBooks)
            {
                ApplyInventoryValuesToBook(book);
            }

            var borrowableBooks = allBooks
                .Where(b => b.IsBorrowable && b.AvailableCopies > 0)
                .ToList();

            model.AllBooks = allBooks;
            model.Books = borrowableBooks;
            model.Users = _context.Users.ToList();

            return View(model);
        }


        public string GenerateReferenceNumber()
        {
            var settings = _context.Settings.FirstOrDefault();
            var shortCode = !string.IsNullOrWhiteSpace(settings?.ShortCode) ? settings.ShortCode : "BNP";

            var existingNumbers = _context.Books
                .Where(b => !string.IsNullOrEmpty(b.ReferenceNumber) && b.ReferenceNumber.StartsWith(shortCode))
                .Select(b => b.ReferenceNumber)
                .ToList();

            int maxNumber = 0;

            foreach (var reference in existingNumbers)
            {
                var numericPart = reference.Substring(shortCode.Length);

                if (int.TryParse(numericPart, out int number) && number > maxNumber)
                {
                    maxNumber = number;
                }
            }

            int nextNumber = maxNumber + 1;
            return $"{shortCode}{nextNumber:D3}";
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin,Librarian")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool exists = await BookExistsAsync(model);

            if (exists)
            {
                ModelState.AddModelError("", "This book already exists. Update the existing record instead of creating a duplicate.");
                return View(model);
            }

            var referenceNumber = GenerateReferenceNumber();
            var photoPath = await SaveBookPhotoAsync(model.Photo);

            var book = new Book
            {
                Title = model.Title?.Trim(),
                Author = model.Author?.Trim(),
                ISBN = model.ISBN?.Trim(),
                TotalCopies = model.TotalCopies,
                AvailableCopies = model.AvailableCopies,
                PhotoPath = photoPath,
                Category = model.Category?.Trim(),
                Edition = model.Edition?.Trim(),
                ShelfNumber = model.ShelfNumber?.Trim(),
                Department = model.Department?.Trim(),
                BorrowingDays = model.BorrowingDays,
                IsBorrowable = model.IsBorrowable,
                ReferenceNumber = referenceNumber
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var currentUser = await _userManager.GetUserAsync(User);
            await GetOrCreateInventoryRecordAsync(book, currentUser?.Id);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Book created successfully.";
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin,Librarian")]
        [HttpGet]
        public IActionResult BorrowedBooks()
        {
            var allLoans = _context.Loans
                .Include(l => l.User)
                .Include(l => l.Book)
                .ToList();

            var books = _context.Books
                .Include(b => b.Loans)
                .Include(b => b.InventoryRecord)
                .Where(b => b.IsBorrowable)
                .ToList();

            foreach (var book in books)
            {
                ApplyInventoryValuesToBook(book);
            }

            books = books.Where(b => b.AvailableCopies > 0).ToList();

            var model = new BorrowBookViewModel
            {
                Loans = allLoans,
                Users = _context.Users.ToList(),
                Books = books,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(2)
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> BorrowedBooks(BookViewModel model)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == model.BookId);
            if (book == null || book.AvailableCopies <= 0)
                return RedirectToAction("BorrowedBooks");

            bool alreadyBorrowed = await _context.Loans.AnyAsync(l =>
                l.BookId == model.BookId &&
                l.UserId == model.StudentId &&
                l.ReturnDate == null);

            if (alreadyBorrowed)
                return RedirectToAction("BorrowedBooks");

            var loan = new Loan
            {
                BookId = model.BookId,
                UserId = model.StudentId,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(int.Parse(book.BorrowingDays)),
                Status = LoanStatus.Borrowed
            };

            book.AvailableCopies -= 1;

            var currentUser = await _userManager.GetUserAsync(User);
            var inventory = await GetOrCreateInventoryRecordAsync(book, currentUser?.Id);
            SyncInventoryFromBook(book, inventory, currentUser?.Id);

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return RedirectToAction("BorrowedBooks");
        }

        [Authorize(Roles = "Admin,Librarian")]
        [HttpGet]
        public IActionResult ReturnedBooks()
        {
            var returnedLoans = _context.Loans
                .Include(l => l.User)
                .Include(l => l.Book)
                .Where(l => l.ReturnDate != null)
                .OrderByDescending(l => l.ReturnDate)
                .ToList();

            var model = new BorrowBookViewModel
            {
                Loans = returnedLoans
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ReturnedBooks(BorrowBookViewModel model)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l =>
                    l.Id == model.LoanId &&
                    l.UserId == model.StudentId &&
                    l.ReturnDate == null);

            if (loan == null)
                return BadRequest("Invalid or already returned loan.");

            var returnDate = DateTime.Now;
            loan.ReturnDate = returnDate;
            loan.Status = LoanStatus.Returned;
            if (returnDate.Date > loan.DueDate.Date)
            {
                int daysLate = (returnDate.Date - loan.DueDate.Date).Days;
                loan.FrozenFineAmount = daysLate * 10;
                loan.FinePaymentStatus = FinePaymentStatus.Unpaid;
            }
            else
            {
                loan.FrozenFineAmount = 0;
                loan.FinePaymentStatus = FinePaymentStatus.None;
            }

            loan.Book.AvailableCopies += 1;
            var currentUser = await _userManager.GetUserAsync(User);
            var inventory = await GetOrCreateInventoryRecordAsync(loan.Book, currentUser?.Id);
            SyncInventoryFromBook(loan.Book, inventory, currentUser?.Id);

            var reservation = await _context.BookReservations
                .Include(r => r.User)
                .Where(r =>
                    r.BookId == loan.BookId &&
                    !r.IsFulfilled &&
                    !r.IsNotified)
                .OrderBy(r => r.ReservedOn)
                .FirstOrDefaultAsync();

            if (reservation != null)
            {
                reservation.IsNotified = true;
                reservation.IsFulfilled = true;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("BorrowedBooks");
        }

        [Authorize(Roles = "Admin,Librarian")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors and try again.";
                return RedirectToAction(nameof(Index));
            }

            bool exists = await BookExistsAsync(model, model.Id);

            if (exists)
            {
                TempData["Error"] = "Another book with the same details already exists.";
                return RedirectToAction(nameof(Index));
            }

            var book = await _context.Books
                .Include(b => b.InventoryRecord)
                .FirstOrDefaultAsync(b => b.Id == model.Id);

            if (book == null)
                return NotFound();

            book.Title = model.Title;
            book.Author = model.Author;
            book.ISBN = model.ISBN;
            book.TotalCopies = model.TotalCopies;
            book.AvailableCopies = model.AvailableCopies;
            book.Category = model.Category;
            book.Edition = model.Edition;
            book.ShelfNumber = model.ShelfNumber;
            book.Department = model.Department;
            book.BorrowingDays = model.BorrowingDays;
            book.IsBorrowable = model.IsBorrowable;

            if (model.Photo != null && model.Photo.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(book.PhotoPath) && book.PhotoPath.StartsWith("/images/covers/"))
                {
                    var oldFilePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        book.PhotoPath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                book.PhotoPath = await SaveBookPhotoAsync(model.Photo);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var inventory = await GetOrCreateInventoryRecordAsync(book, currentUser?.Id);
            SyncInventoryFromBook(book, inventory, currentUser?.Id);

            _context.Update(book);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Book updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Librarian")]
        [HttpGet]
        public IActionResult Details(int id)
        {
            var book = _context.Books.FirstOrDefault(g => g.Id == id);
            return View(book);
        }

        [HttpPost]
        public IActionResult Details()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(g => g.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            var book = await _context.Books
                .Include(b => b.InventoryRecord)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            if (book.InventoryRecord != null)
            {
                _context.InventoryRecords.Remove(book.InventoryRecord);
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        
        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task <IActionResult> MyBorrowedBooks(string UserId)

        {
            var loggedInUser = await _userManager.GetUserAsync(User);
            if (loggedInUser!= null)
            {
                var loan = _context.Loans.Include(l => l.Book).Where(u => u.UserId == loggedInUser.Id).ToList();

                return View(loan);
            }

            return View();
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> MyHistory()
        {
            var loggedInUser = await _userManager.GetUserAsync(User);

            if (loggedInUser != null)
            {
                var allLoans = await _context.Loans
                    .Include(l => l.Book)
                    .Where(l => l.UserId == loggedInUser.Id)
                    .ToListAsync();

                return View(allLoans); 
            }

            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "Please upload an Excel file." });

            int imported = 0;
            int skipped = 0;

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                var newBooks = new List<Book>();

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    string title = row.Cell(1).GetString().Trim();
                    string author = row.Cell(2).GetString().Trim();
                    string isbn = row.Cell(3).GetString().Trim();
                    string edition = row.Cell(4).GetString().Trim();
                    string department = row.Cell(5).GetString().Trim();
                    string category = row.Cell(6).GetString().Trim();
                    string shelfNumber = row.Cell(7).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(shelfNumber))
                    {
                        skipped++;
                        continue;
                    }

                    if (!row.Cell(8).TryGetValue(out int totalCopies))
                    {
                        skipped++;
                        continue;
                    }

                    string borrowingDays = row.Cell(9).GetString().Trim();
                    bool exists = _context.Books.Any(b =>
                     b.Title.ToLower() == title.ToLower() &&
                     b.Author.ToLower() == author.ToLower() &&
                     b.ISBN.ToLower() == isbn.ToLower() &&
                     b.Edition.ToLower() == edition.ToLower() &&
                     b.Department.ToLower() == department.ToLower() &&
                     b.Category.ToLower() == category.ToLower() &&
                     b.ShelfNumber.ToLower() == shelfNumber.ToLower()
 );

                    if (exists)
                    {
                        skipped++;
                        continue;
                    }

                    var book = new Book
                    {
                        Title = title,
                        Author = author,
                        ISBN = isbn,
                        Edition = edition,
                        Department = department,
                        Category = category,
                        ShelfNumber = shelfNumber,
                        TotalCopies = totalCopies,
                        AvailableCopies = totalCopies,
                        BorrowingDays = borrowingDays,
                        IsBorrowable = true,
                        ReferenceNumber = GenerateReferenceNumber()
                    };

                    _context.Books.Add(book);
                    newBooks.Add(book);
                    imported++;
                }

                await _context.SaveChangesAsync();
                var currentUser = await _userManager.GetUserAsync(User);

                foreach (var book in newBooks)
                {
                    await GetOrCreateInventoryRecordAsync(book, currentUser?.Id);
                }
                await _context.SaveChangesAsync();
                return Json(new
                {
                    success = true,
                    message = $"Import complete. {imported} books added, {skipped} skipped."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Import failed: " + ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Reserve(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
                return Json(new { success = false, message = "Book not found." });

            var inventory = await _context.InventoryRecords
                .FirstOrDefaultAsync(i => i.BookId == bookId);
           var availableCopies = inventory != null ? inventory.AvailableCopies : book.AvailableCopies;

            if (availableCopies > 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Book is available. You can borrow it directly."
                });
            }

            bool alreadyReserved = await _context.BookReservations.AnyAsync(r =>
                r.BookId == bookId &&
                r.UserId == user.Id &&
                !r.IsFulfilled);

            if (alreadyReserved)
                return Json(new { success = false, message = "You already have a pending reservation." });

            _context.BookReservations.Add(new BookReservation
            {
                BookId = bookId,
                UserId = user.Id
            });

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Reservation placed. You will be notified when available." });
        }


        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Reservations()
        {
            var reservations = _context.BookReservations
                .Include(r => r.Book)
                .Include(r => r.User)
                .OrderByDescending(r => r.ReservedOn)
                .ToList();

            return View(reservations);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyReservations()
        {
            var user = await _userManager.GetUserAsync(User);

            var reservations = _context.BookReservations
                .Include(r => r.Book)
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.ReservedOn)
                .ToList();
            return View(reservations);
        }

        
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SuggestBook(BookSuggestion model)
        {
            model.SuggestedAt = DateTime.Now;
            model.SuggestedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _context.BookSuggestions.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MySuggestions()
        {
            var user = await _userManager.GetUserAsync(User);

            var suggestions = await _context.BookSuggestions
                .Where(s => s.SuggestedByUserId == user.Id)
                .OrderByDescending(s => s.SuggestedOn)
                .ToListAsync();

            return View(suggestions);
        }

        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult BookSuggestions()
        {
            var suggestions = _context.BookSuggestions
                .Include(s => s.SuggestedBy)
                .OrderByDescending(s => s.SuggestedOn)
                .ToList();

            return View(suggestions);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveSuggestion(int id)
        {
            var suggestion = await _context.BookSuggestions.FindAsync(id);
            if (suggestion == null)
                return NotFound();

            var model = new ApproveSuggestionViewModel
            {
                SuggestionId = suggestion.Id,
                Title = suggestion.Title,
                Author = suggestion.Author,
                ISBN = suggestion.ISBN,
                BorrowingDays = "5",
                TotalCopies = 1,
                AvailableCopies = 1,
                IsBorrowable = true
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveSuggestion(ApproveSuggestionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool exists = await _context.Books.AnyAsync(b =>
                b.Title.ToLower() == model.Title.Trim().ToLower() &&
                b.Author.ToLower() == model.Author.Trim().ToLower() &&
                b.ISBN.ToLower() == model.ISBN.Trim().ToLower() &&
                b.Edition.ToLower() == model.Edition.Trim().ToLower() &&
                b.Department.ToLower() == model.Department.Trim().ToLower() &&
                b.Category.ToLower() == model.Category.Trim().ToLower() &&
                b.ShelfNumber.ToLower() == model.ShelfNumber.Trim().ToLower()
            );

            if (exists)
            {
                ModelState.AddModelError("", "This book already exists in the system.");
                return View(model);
            }

            var suggestion = await _context.BookSuggestions.FindAsync(model.SuggestionId);
            if (suggestion == null)
                return NotFound();

            var book = new Book
            {
                Title = model.Title,
                Author = model.Author,
                ISBN = model.ISBN,
                BorrowingDays = model.BorrowingDays,
                ShelfNumber = model.ShelfNumber,
                TotalCopies = model.TotalCopies,
                AvailableCopies = model.AvailableCopies,
                Category = model.Category,
                Department = model.Department,
                Edition = model.Edition,
                IsBorrowable = model.IsBorrowable,
                ReferenceNumber = GenerateReferenceNumber()
            };

            _context.Books.Add(book);
            suggestion.Approved = true;

            await _context.SaveChangesAsync();

            var currentUser = await _userManager.GetUserAsync(User);
            await GetOrCreateInventoryRecordAsync(book, currentUser?.Id);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Suggestion approved and added to the library successfully.";
            return RedirectToAction("BookSuggestions");
        }
    }

    }

