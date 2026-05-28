using LibraryManagementSystem.Models;
using LibraryManagementSystem.Models.CDACC;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin,Librarian")]
    public class CompetencyAdminController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompetencyAdminController(
            LibraryDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        // STANDARDS
        

        public async Task<IActionResult> Standards()
        {
            var standards = await _context.CompetencyStandards
                .OrderBy(c => c.Program)
                .ThenBy(c => c.Level)
                .ThenBy(c => c.Semester)
                .ThenBy(c => c.UnitCode)
                .ToListAsync();

            return View(standards);
        }

        public IActionResult CreateStandard()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStandard(CompetencyStandard model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Program = model.Program?.Trim();
            model.Level = model.Level?.Trim();
            model.UnitCode = model.UnitCode?.Trim();
            model.UnitName = model.UnitName?.Trim();

            model.VersionNumber = 1;
            model.EffectiveFrom = DateTime.UtcNow;
            model.IsActive = true;

            _context.CompetencyStandards.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Standards));
        }

        // Create new version of a standard
        public async Task<IActionResult> NewVersion(int id)
        {
            var existing = await _context.CompetencyStandards.FindAsync(id);
            if (existing == null)
                return NotFound();

            // deactivate old
            existing.IsActive = false;
            existing.EffectiveTo = DateTime.Now;

            var newVersion = new CompetencyStandard
            {
                Program = existing.Program,
                Level = existing.Level,
                Semester = existing.Semester,
                UnitCode = existing.UnitCode,
                UnitName = existing.UnitName,
                VersionNumber = existing.VersionNumber + 1,
                EffectiveFrom = DateTime.UtcNow,
                IsActive = true
            };

            _context.CompetencyStandards.Add(newVersion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Standards));
        }

        // =========================
        // RESOURCES
        // =========================

        public async Task<IActionResult> Resources()
        {
            var resources = await _context.LibraryResources
                .Include(r => r.ResourceCompetencies)
                .OrderBy(r => r.Title)
                .ToListAsync();

            return View(resources);
        }

        public IActionResult CreateResource()

        {
            return View();
        }

        private bool IsValidIsbn(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return false;

            var clean = isbn.Replace("-", "").Replace(" ", "");

            return (clean.Length == 10 || clean.Length == 13)
                   && clean.All(char.IsDigit);
        }

        private async Task<string> GenerateReferenceNumber(ResourceType type)
        {
            string prefix = type switch
            {
                ResourceType.Book => "BOOK",
                ResourceType.Document => "DOC",
                ResourceType.PastPaper => "PP",
                ResourceType.YouTube => "VID",
                ResourceType.WebLink => "WEB",
                _ => "RES"
            };

            var count = await _context.LibraryResources.CountAsync(r => r.Type == type);
            var nextNumber = count + 1;

            return $"{prefix}-{nextNumber:D4}";
        }
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        
      
        public async Task<IActionResult> CreateResource(ResourceCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string storedPath = null;
            string coverPath = null;

            // Only Document, Book, and PastPaper allow PDF uploads
            if (model.Type != ResourceType.Document &&
                model.Type != ResourceType.Book &&
                model.Type != ResourceType.PastPaper &&
                model.UploadedFile != null)
            {
                ModelState.AddModelError("", "File uploads are only allowed for Document, Book, and PastPaper types.");
                return View(model);
            }

            if (model.Type == ResourceType.Document)
            {
                if (model.UploadedFile == null || model.UploadedFile.Length == 0)
                {
                    ModelState.AddModelError("UploadedFile", "Document upload is required.");
                    return View(model);
                }

                var extension = Path.GetExtension(model.UploadedFile.FileName).ToLower();

                if (extension != ".pdf")
                {
                    ModelState.AddModelError("UploadedFile", "Only PDF files are allowed.");
                    return View(model);
                }

                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "documents");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + extension;
                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.UploadedFile.CopyToAsync(stream);
                }

                storedPath = "/uploads/documents/" + fileName;
            }
            else if (model.Type == ResourceType.Book)
            {
                if (string.IsNullOrWhiteSpace(model.Author))
                {
                    ModelState.AddModelError("Author", "Author is required for Book type.");
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(model.ISBN) && !IsValidIsbn(model.ISBN))
                {
                    ModelState.AddModelError("ISBN", "ISBN must be 10 or 13 digits.");
                    return View(model);
                }

                if (model.UploadedFile == null || model.UploadedFile.Length == 0)
                {
                    ModelState.AddModelError("UploadedFile", "Book PDF upload is required.");
                    return View(model);
                }

                var extension = Path.GetExtension(model.UploadedFile.FileName).ToLower();

                if (extension != ".pdf")
                {
                    ModelState.AddModelError("UploadedFile", "Books must be uploaded as PDF files.");
                    return View(model);
                }

                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "books");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + extension;
                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.UploadedFile.CopyToAsync(stream);
                }

                storedPath = "/uploads/books/" + fileName;

                if (model.CoverImage != null && model.CoverImage.Length > 0)
                {
                    var coverExtension = Path.GetExtension(model.CoverImage.FileName).ToLower();

                    if (coverExtension != ".jpg" && coverExtension != ".jpeg" && coverExtension != ".png")
                    {
                        ModelState.AddModelError("CoverImage", "Cover image must be JPG or PNG.");
                        return View(model);
                    }

                    var coverFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        "covers");

                    if (!Directory.Exists(coverFolder))
                        Directory.CreateDirectory(coverFolder);

                    var coverFileName = Guid.NewGuid() + coverExtension;
                    var coverFullPath = Path.Combine(coverFolder, coverFileName);

                    using (var stream = new FileStream(coverFullPath, FileMode.Create))
                    {
                        await model.CoverImage.CopyToAsync(stream);
                    }

                    coverPath = "/uploads/covers/" + coverFileName;
                }
            }
            else if (model.Type == ResourceType.PastPaper)
            {
                if (model.UploadedFile == null || model.UploadedFile.Length == 0)
                {
                    ModelState.AddModelError("UploadedFile", "Past paper PDF upload is required.");
                    return View(model);
                }

                var extension = Path.GetExtension(model.UploadedFile.FileName).ToLower();

                if (extension != ".pdf")
                {
                    ModelState.AddModelError("UploadedFile", "Past papers must be PDF files.");
                    return View(model);
                }

                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "pastpapers");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + extension;
                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.UploadedFile.CopyToAsync(stream);
                }

                storedPath = "/uploads/pastpapers/" + fileName;
            }
            else if (model.Type == ResourceType.WebLink)
            {
                if (string.IsNullOrWhiteSpace(model.ExternalUrl))
                {
                    ModelState.AddModelError("ExternalUrl", "External URL is required.");
                    return View(model);
                }

                if (!Uri.TryCreate(model.ExternalUrl, UriKind.Absolute, out var uriResult))
                {
                    ModelState.AddModelError("ExternalUrl", "Invalid URL format.");
                    return View(model);
                }

                storedPath = model.ExternalUrl.Trim();
            }
            else if (model.Type == ResourceType.YouTube)
            {
                if (string.IsNullOrWhiteSpace(model.ExternalUrl))
                {
                    ModelState.AddModelError("ExternalUrl", "YouTube URL is required.");
                    return View(model);
                }

                if (!Uri.TryCreate(model.ExternalUrl, UriKind.Absolute, out var uriResult))
                {
                    ModelState.AddModelError("ExternalUrl", "Invalid YouTube URL.");
                    return View(model);
                }

                if (!uriResult.Host.Contains("youtube") && !uriResult.Host.Contains("youtu.be"))
                {
                    ModelState.AddModelError("ExternalUrl", "URL must be a valid YouTube link.");
                    return View(model);
                }

                storedPath = model.ExternalUrl.Trim();
            }
            else
            {
                ModelState.AddModelError("", "Invalid resource type.");
                return View(model);
            }

            var reference = await GenerateReferenceNumber(model.Type);

            var resource = new LibraryResource
            {
                Title = model.Title,
                Type = model.Type,
                UrlOrFilePath = storedPath,
                Author = model.Type == ResourceType.Book ? model.Author : null,
                ISBN = model.Type == ResourceType.Book ? model.ISBN : null,
                CoverImagePath = model.Type == ResourceType.Book ? coverPath : null,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow,
                ReferenceNumber = reference
            };

            _context.LibraryResources.Add(resource);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MapResource), new { resourceId = resource.Id });

        }

        public async Task<IActionResult> EditResource(int id)
        {
            var resource = await _context.LibraryResources.FindAsync(id);
            if (resource == null)
                return NotFound();

            var vm = new ResourceCreateViewModel
            {
                Title = resource.Title,
                Type = resource.Type,
                ExternalUrl = resource.Type == ResourceType.WebLink || resource.Type == ResourceType.YouTube
                    ? resource.UrlOrFilePath
                    : null,
                Author = resource.Author,
                ISBN = resource.ISBN,
                IsActive = resource.IsActive
            };

            ViewBag.ResourceId = resource.Id;
            ViewBag.ExistingFilePath = resource.UrlOrFilePath;
            ViewBag.ExistingCoverPath = resource.CoverImagePath;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditResource(int id, ResourceCreateViewModel model)
        {
            var resource = await _context.LibraryResources.FindAsync(id);
            if (resource == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.ResourceId = resource.Id;
                ViewBag.ExistingFilePath = resource.UrlOrFilePath;
                ViewBag.ExistingCoverPath = resource.CoverImagePath;
                return View(model);
            }

            string storedPath = resource.UrlOrFilePath;
            string coverPath = resource.CoverImagePath;

            if (model.Type != ResourceType.Document &&
                model.Type != ResourceType.Book &&
                model.Type != ResourceType.PastPaper &&
                model.UploadedFile != null)
            {
                ModelState.AddModelError("", "File uploads are only allowed for Document, Book, and PastPaper types.");
                ViewBag.ResourceId = resource.Id;
                ViewBag.ExistingFilePath = resource.UrlOrFilePath;
                ViewBag.ExistingCoverPath = resource.CoverImagePath;
                return View(model);
            }

            if (model.Type == ResourceType.Document)
            {
                if (model.UploadedFile != null && model.UploadedFile.Length > 0)
                {
                    var extension = Path.GetExtension(model.UploadedFile.FileName).ToLower();
                    if (extension != ".pdf")
                    {
                        ModelState.AddModelError("UploadedFile", "Only PDF files are allowed.");
                        ViewBag.ResourceId = resource.Id;
                        ViewBag.ExistingFilePath = resource.UrlOrFilePath;
                        ViewBag.ExistingCoverPath = resource.CoverImagePath;
                        return View(model);
                    }

                    var uploadsFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        "documents");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid() + extension;
                    var fullPath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await model.UploadedFile.CopyToAsync(stream);
                    }

                    storedPath = "/uploads/documents/" + fileName;
                }

                resource.Author = null;
                resource.ISBN = null;
                resource.CoverImagePath = null;
            }
            else if (model.Type == ResourceType.Book)
            {
                if (string.IsNullOrWhiteSpace(model.Author))
                {
                    ModelState.AddModelError("Author", "Author is required for Book type.");
                    ViewBag.ResourceId = resource.Id;
                    ViewBag.ExistingFilePath = resource.UrlOrFilePath;
                    ViewBag.ExistingCoverPath = resource.CoverImagePath;
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(model.ISBN) && !IsValidIsbn(model.ISBN))
                {
                    ModelState.AddModelError("ISBN", "ISBN must be 10 or 13 digits.");
                    ViewBag.ResourceId = resource.Id;
                    ViewBag.ExistingFilePath = resource.UrlOrFilePath;
                    ViewBag.ExistingCoverPath = resource.CoverImagePath;
                    return View(model);
                }

                if (model.UploadedFile != null && model.UploadedFile.Length > 0)
                {
                    var extension = Path.GetExtension(model.UploadedFile.FileName).ToLower();
                    if (extension != ".pdf")
                    {
                        ModelState.AddModelError("UploadedFile", "Books must be uploaded as PDF files.");
                        ViewBag.ResourceId = resource.Id;
                        ViewBag.ExistingFilePath = resource.UrlOrFilePath;
                        ViewBag.ExistingCoverPath = resource.CoverImagePath;
                        return View(model);
                    }

                    var uploadsFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        "books");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid() + extension;
                    var fullPath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await model.UploadedFile.CopyToAsync(stream);
                    }

                    storedPath = "/uploads/books/" + fileName;
                }

                if (model.CoverImage != null && model.CoverImage.Length > 0)
                {
                    var coverExtension = Path.GetExtension(model.CoverImage.FileName).ToLower();
                    if (coverExtension != ".jpg" && coverExtension != ".jpeg" && coverExtension != ".png")
                    {
                        ModelState.AddModelError("CoverImage", "Cover image must be JPG or PNG.");
                        ViewBag.ResourceId = resource.Id;
                        ViewBag.ExistingFilePath = resource.UrlOrFilePath;
                        ViewBag.ExistingCoverPath = resource.CoverImagePath;
                        return View(model);
                    }

                    var coverFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        "covers");

                    if (!Directory.Exists(coverFolder))
                        Directory.CreateDirectory(coverFolder);

                    var coverFileName = Guid.NewGuid() + coverExtension;
                    var coverFullPath = Path.Combine(coverFolder, coverFileName);

                    using (var stream = new FileStream(coverFullPath, FileMode.Create))
                    {
                        await model.CoverImage.CopyToAsync(stream);
                    }

                    coverPath = "/uploads/covers/" + coverFileName;
                }

                resource.Author = model.Author;
                resource.ISBN = model.ISBN;
                resource.CoverImagePath = coverPath;
            }
            else if (model.Type == ResourceType.PastPaper)
            {
                if (model.UploadedFile != null && model.UploadedFile.Length > 0)
                {
                    var extension = Path.GetExtension(model.UploadedFile.FileName).ToLower();
                    if (extension != ".pdf")
                    {
                        ModelState.AddModelError("UploadedFile", "Past papers must be PDF files.");
                        ViewBag.ResourceId = resource.Id;
                        ViewBag.ExistingFilePath = resource.UrlOrFilePath;
                        ViewBag.ExistingCoverPath = resource.CoverImagePath;
                        return View(model);
                    }

                    var uploadsFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        "pastpapers");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid() + extension;
                    var fullPath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await model.UploadedFile.CopyToAsync(stream);
                    }

                    storedPath = "/uploads/pastpapers/" + fileName;
                }

                resource.Author = null;
                resource.ISBN = null;
                resource.CoverImagePath = null;
            }
            else if (model.Type == ResourceType.WebLink)
            {
                if (string.IsNullOrWhiteSpace(model.ExternalUrl))
                {
                    ModelState.AddModelError("ExternalUrl", "External URL is required.");
                    ViewBag.ResourceId = resource.Id;
                    ViewBag.ExistingFilePath = resource.UrlOrFilePath;
                    ViewBag.ExistingCoverPath = resource.CoverImagePath;
                    return View(model);
                }

                if (!Uri.TryCreate(model.ExternalUrl, UriKind.Absolute, out _))
                {
                    ModelState.AddModelError("ExternalUrl", "Invalid URL format.");
                    ViewBag.ResourceId = resource.Id;
                    ViewBag.ExistingFilePath = resource.UrlOrFilePath;
                    ViewBag.ExistingCoverPath = resource.CoverImagePath;
                    return View(model);
                }

                storedPath = model.ExternalUrl.Trim();
                resource.Author = null;
                resource.ISBN = null;
                resource.CoverImagePath = null;
            }
            else if (model.Type == ResourceType.YouTube)
            {
                if (string.IsNullOrWhiteSpace(model.ExternalUrl))
                {
                    ModelState.AddModelError("ExternalUrl", "YouTube URL is required.");
                    ViewBag.ResourceId = resource.Id;
                    ViewBag.ExistingFilePath = resource.UrlOrFilePath;
                    ViewBag.ExistingCoverPath = resource.CoverImagePath;
                    return View(model);
                }

                if (!Uri.TryCreate(model.ExternalUrl, UriKind.Absolute, out var uriResult))
                {
                    ModelState.AddModelError("ExternalUrl", "Invalid YouTube URL.");
                    ViewBag.ResourceId = resource.Id;
                    ViewBag.ExistingFilePath = resource.UrlOrFilePath;
                    ViewBag.ExistingCoverPath = resource.CoverImagePath;
                    return View(model);
                }

                if (!uriResult.Host.Contains("youtube") && !uriResult.Host.Contains("youtu.be"))
                {
                    ModelState.AddModelError("ExternalUrl", "URL must be a valid YouTube link.");
                    ViewBag.ResourceId = resource.Id;
                    ViewBag.ExistingFilePath = resource.UrlOrFilePath;
                    ViewBag.ExistingCoverPath = resource.CoverImagePath;
                    return View(model);
                }

                storedPath = model.ExternalUrl.Trim();
                resource.Author = null;
                resource.ISBN = null;
                resource.CoverImagePath = null;
            }

            resource.Title = model.Title;
            resource.Type = model.Type;
            resource.UrlOrFilePath = storedPath;
            resource.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Resources));
        }

        public async Task<IActionResult> DeleteResource(int id)
        {
            var resource = await _context.LibraryResources
                .FirstOrDefaultAsync(r => r.Id == id);

            if (resource == null)
                return NotFound();

            return View(resource);
        }

        [HttpPost, ActionName("DeleteResource")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteResourceConfirmed(int id)
        {
            var resource = await _context.LibraryResources
                .Include(r => r.ResourceCompetencies)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (resource == null)
                return NotFound();

            if (resource.ResourceCompetencies != null && resource.ResourceCompetencies.Any())
            {
                _context.ResourceCompetencies.RemoveRange(resource.ResourceCompetencies);
            }

            _context.LibraryResources.Remove(resource);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Resources));
        }
        // MAP RESOURCE

        public async Task<IActionResult> MapResource(int resourceId)
        {
            var resource = await _context.LibraryResources
                .Include(r => r.ResourceCompetencies)
                .FirstOrDefaultAsync(r => r.Id == resourceId);

            if (resource == null)
                return NotFound();

            var standards = await _context.CompetencyStandards
                .Where(c => c.IsActive)
                .OrderBy(c => c.Program)
                .ThenBy(c => c.Level)
                .ThenBy(c => c.Semester)
                .ToListAsync();

            var vm = new MapResourceViewModel
            {
                Resource = resource,
                Standards = standards,
                SelectedStandards = resource.ResourceCompetencies
                    .Where(rc => rc.IsActive)
                    .Select(rc => rc.CompetencyStandardId)
                    .ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MapResource(int resourceId, List<int> selectedStandards)
        {
            var resource = await _context.LibraryResources
                .Include(r => r.ResourceCompetencies)
                .FirstOrDefaultAsync(r => r.Id == resourceId);

            if (resource == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var validStandardIds = await _context.CompetencyStandards
                .Where(c => c.IsActive)
                .Select(c => c.Id)
                .ToListAsync();

            var selected = (selectedStandards ?? new List<int>())
                .Where(id => validStandardIds.Contains(id))
                .Distinct()
                .ToList();

            foreach (var existing in resource.ResourceCompetencies)
            {
                existing.IsActive = selected.Contains(existing.CompetencyStandardId);
            }

            var existingIds = resource.ResourceCompetencies
                .Select(rc => rc.CompetencyStandardId)
                .ToList();

            foreach (var standardId in selected)
            {
                if (!existingIds.Contains(standardId))
                {
                    _context.ResourceCompetencies.Add(new ResourceCompetency
                    {
                        ResourceId = resourceId,
                        CompetencyStandardId = standardId,
                        MappedByUserId = currentUser.Id,
                        MappedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Resources));
        }
    }
}