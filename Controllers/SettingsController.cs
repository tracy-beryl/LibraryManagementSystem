using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            var mySettings = _context.Settings.FirstOrDefault();
            return View(mySettings);
        }

        [HttpPost]
        public IActionResult Index (Settings model, IFormFile formFile)
        {
            if (ModelState.IsValid)
            {
                string logoFileName = null;


                if (formFile!= null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    Directory.CreateDirectory(uploadsFolder);
                    logoFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
                    var logoPath = Path.Combine(uploadsFolder, logoFileName);
                    formFile.CopyTo(new FileStream(logoPath, FileMode.Create));
                }



                var settings = new Settings
                {
                    Name = model.Name,
                    ShortCode = model.ShortCode,
                    LogoPath = logoFileName,
               
                };

                _context.Settings.Add(settings);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }


    }
  }

