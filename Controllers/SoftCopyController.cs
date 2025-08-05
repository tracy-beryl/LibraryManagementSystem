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
    public class SoftCopyController : Controller
    {
        private readonly LibraryDbContext _context;
        

        public SoftCopyController(LibraryDbContext context)
        {
            _context = context;
            
        }

        public IActionResult Index()
        {
            var files = _context.SoftCopies.OrderByDescending(s => s.UploadDate).ToList();
            return View(files);
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(SoftCopyViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                // file Upload
                if (model.File != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/softcopy");
                    Directory.CreateDirectory(uploadsFolder);
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.File.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    model.File.CopyTo(new FileStream(filePath, FileMode.Create));
                }

               

                var softCopy = new SoftCopy
                {
                    Title = model.Title,
                    Description = model.Description,
                    FilePath = "/softcopies/" + uniqueFileName,
                    UploadedBy = User.Identity.Name
                };

                _context.SoftCopies.Add(softCopy);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }

}
