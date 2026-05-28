using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace LibraryManagementSystem.Controllers
{
    public class SettingsController : Controller
    {
        private readonly LibraryDbContext _context;

        public SettingsController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var mySettings = _context.Settings.FirstOrDefault() ?? new Settings();
            return View(mySettings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Settings model, IFormFile formFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            var settings = _context.Settings.FirstOrDefault();

            if (settings == null)
            {
                settings = new Settings();
                _context.Settings.Add(settings);
            }

            settings.Name = model.Name;
            settings.ShortCode = model.ShortCode;

            if (formFile != null && formFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                Directory.CreateDirectory(uploadsFolder);

                var logoFileName = Guid.NewGuid() + "_" + Path.GetFileName(formFile.FileName);
                var logoPath = Path.Combine(uploadsFolder, logoFileName);

                using (var stream = new FileStream(logoPath, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }

                settings.LogoPath = logoFileName;
            }

            _context.SaveChanges();

            TempData["Success"] = "Settings saved successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}