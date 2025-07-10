
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace LibraryManagementSystem.Controllers
{
    
    public class BookController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookController(LibraryDbContext libraryDbContext, UserManager<ApplicationUser> userManager)
        {
            _context = libraryDbContext;
            _userManager = userManager;
        }


        public ActionResult Index()
        {
            var model = new BookViewModel();

            
            var books = _context.Books.ToList();

            
            var activeLoans = _context.Loans
                .Where(c => c.ReturnDate == null && c.BookId != null)
                .ToList();


            var loanCounts = activeLoans
                .GroupBy(l => l.BookId)
                .Select(g => new { BookId = g.Key, Count = g.Count() })
                .ToDictionary(g => g.BookId, g => g.Count);

           
            foreach (var book in books)
            {
                int borrowedCount = loanCounts.ContainsKey(book.Id) ? loanCounts[book.Id] : 0;
                book.AvailableCopies = book.TotalCopies - borrowedCount;
            }

            model.Books = books;
            model.Users = _context.Users.ToList();

            return View(model);
        }

        


        [HttpGet]
        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateBookViewModel model)
        {
            if (ModelState.IsValid)
            {

                string uniqueFileName = null;
                    if (model.Photo != null)
                    {

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    Directory.CreateDirectory(uploadsFolder);

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        
                        model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                    var book = new Book
                    {
                    Title = model.Title,
                    Author = model.Author,
                    ISBN = model.ISBN,
                    TotalCopies = model.TotalCopies,
                    AvailableCopies = model.AvailableCopies,
                    PhotoPath =uniqueFileName,
                    Category = model.Category,
                    BorrowingDays = model.BorrowingDays

                };
                _context.Books.Add(book);
                _context.SaveChanges();
                RedirectToAction("Index");
            }
            return View();
        }


        [HttpGet]
        
        public IActionResult BorrowedBooks()
        {
            var allLoans = _context.Loans
                 .Include(l => l.User)
                 .Include(l => l.Book)
                .ToList();
           
          

            var model = new BorrowBookViewModel
            {
                Loans = allLoans,
                Users = _context.Users.ToList(),
                Books = _context.Books.ToList(),
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(2)
            
            };
                  return View(model);
        }

        [HttpPost]
        public IActionResult BorrowedBooks(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                var book = _context.Books.FirstOrDefault(g => g.Id == model.BookId);
                if (book != null && book.AvailableCopies > 0)
                {

                    

                    var loan = new Loan
                    {
                        BookId = model.BookId,
                        UserId = model.StudentId,
                        LoanDate = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(int.Parse(book.BorrowingDays))
                    };

                    _context.Loans.Add(loan);
                    _context.SaveChanges();

                    return RedirectToAction("BorrowedBooks");


                }
            }
            
            return View(model);
        }


        [HttpGet]
        
        public IActionResult ReturnedBooks()
        {
            var returnedLoans = _context.Loans
                .Include(l => l.User)
                .Include(l => l.Book)
                .Where(l => l.ReturnDate != null)
                .ToList();

            var model = new BorrowBookViewModel
            {
                Loans = returnedLoans
            };

            return View(model);
        }



      

        [HttpPost]

        public IActionResult ReturnedBooks(BorrowBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loan = _context.Loans
                    .FirstOrDefault(l => l.Id == model.LoanId && l.UserId == model.StudentId);

                if (loan != null)
                {
                    loan.ReturnDate = DateTime.Now;
                    _context.SaveChanges();

                    var book = _context.Books.FirstOrDefault(b => b.Id == loan.BookId);
                    if (book != null)
                    {
                        book.AvailableCopies += 1;
                        _context.Books.Update(book);
                    }
                }

                return RedirectToAction("BorrowedBooks");
            }

            model.Loans = _context.Loans
            .Include(l => l.User)
            .Include(l => l.Book)
            .ToList();
            model.Users = _context.Users.ToList();
            model.Books = _context.Books.ToList();

            return View("BorrowedBooks", model);
        }


    

        [HttpGet]
        
        public IActionResult Edit(int id)
        {
            var book = _context.Books.FirstOrDefault(g => g.Id == id);

            return View(book);

        }
        [HttpPost]
        public async Task<IActionResult> Edit(CreateBookViewModel model)
        {
            var book = await _context.Books.FirstOrDefaultAsync(g => g.Id == model.Id);
            if (book != null)
            {
                book.Author = model.Author;
                book.ISBN = model.ISBN;
                book.Title = model.Title;
                book.AvailableCopies = model.AvailableCopies;
                book.Category = model.Category;
                book.TotalCopies = model.TotalCopies;
                book.BorrowingDays = model.BorrowingDays;

                
                if (model.Photo != null && model.Photo.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    Directory.CreateDirectory(uploadsFolder); 
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));

                    book.PhotoPath =  uniqueFileName;
                }

                _context.Update(book);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
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


        [HttpPost,ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

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






    }
}
