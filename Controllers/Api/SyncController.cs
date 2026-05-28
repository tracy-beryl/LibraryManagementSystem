using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SyncController(LibraryDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("books")]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _context.Books.AsNoTracking().ToListAsync();
            return Ok(books);
        }

        [HttpGet("pastpapers")]
        public async Task<IActionResult> GetPastPapers()
        {
            var papers = await _context.PastPapers.AsNoTracking().ToListAsync();
            return Ok(papers);
        }

        [HttpPost("uploadpastpaper")]
        public async Task<IActionResult> UploadPastPaper([FromForm] string CourseCode,
                                                         [FromForm] string CourseTitle,
                                                         [FromForm] string AcademicYear,
                                                         [FromForm] string Semester,
                                                         [FromForm] string UploadedBy,
                                                         [FromForm] IFormFile File)
        {
            if (File == null || File.Length == 0)
                return BadRequest("File missing");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "papers");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + File.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await File.CopyToAsync(stream);
            }

            var paper = new PastPapers
            {
                CourseCode = CourseCode,
                CourseTitle = CourseTitle,
                AcademicYear = AcademicYear,
                Semester = Semester,
                UploadedBy = UploadedBy ?? "OfflineUser",
                FilePath = "/papers/" + uniqueFileName
            };

            _context.PastPapers.Add(paper);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, id = paper.Id });
        }

        [HttpPost("uploadbook")]
        public async Task<IActionResult> UploadBook([FromForm] string Title,
                                                    [FromForm] string Author,
                                                    [FromForm] int TotalCopies,
                                                    [FromForm] IFormFile Photo)
        {
            string photoFileName = null;

            if (Photo != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(uploadsFolder);
                photoFileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
                var filePath = Path.Combine(uploadsFolder, photoFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(stream);
                }
            }

            var book = new Book
            {
                Title = Title,
                Author = Author,
                TotalCopies = TotalCopies,
                AvailableCopies = TotalCopies,
                PhotoPath = photoFileName,
                ReferenceNumber = "OFF" + DateTime.UtcNow.Ticks
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, id = book.Id });
        }
    }
}