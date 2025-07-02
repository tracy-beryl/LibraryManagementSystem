using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    public class PastPapersController : Controller
    {
        private readonly LibraryDbContext _context;

        public PastPapersController(LibraryDbContext libraryDbContext)
        {
            _context = libraryDbContext;
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PastPapers()
        {
            var papers = _context.PastPapers.ToList();
            return View(papers);
        }

        [HttpPost]
        public async Task<IActionResult> PastPapers(PastPapersViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                if (model.File != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/papers");
                    Directory.CreateDirectory(uploadsFolder);

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.File.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.File.CopyToAsync(stream);
                    }
                }

                var pastPaper = new PastPapers
                {
                    CourseCode = model.CourseCode,
                    CourseTitle = model.CourseTitle,
                    AcademicYear = model.AcademicYear,
                    Semester = model.Semester,
                    UploadedBy = User.Identity.Name ?? "Admin",
                    FilePath = "/papers/" + uniqueFileName
                };

                _context.PastPapers.Add(pastPaper);
                await _context.SaveChangesAsync();

                return RedirectToAction("PastPapers");
            }

            return View(model);
        }

    }
}
