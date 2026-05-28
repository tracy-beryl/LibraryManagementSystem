using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.IO.Compression;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LibraryManagementSystem.Controllers
{
   
    public class PastPapersController : Controller
    {
        private readonly LibraryDbContext _context;

        public PastPapersController(LibraryDbContext libraryDbContext)
        {
            _context = libraryDbContext;
        }

        [HttpGet]
        [Authorize(Roles = "Librarian,Admin,Student,Lecturer")]
        public IActionResult PastPapers(string category, string search)
        {
            var papers = _context.PastPapers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(category) &&
                Enum.TryParse<PastPaperCategory>(category, true, out var parsedCategory))
            {
                papers = papers.Where(x => x.Category == parsedCategory);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                papers = papers.Where(x =>
                    x.CourseCode.Contains(search) ||
                    x.CourseTitle.Contains(search) ||
                    x.AcademicYear.Contains(search) ||
                    x.Semester.Contains(search));
            }

            var result = papers
                .OrderByDescending(x => x.UploadDate)
                .ToList();

            ViewBag.SelectedCategory = category;
            ViewBag.Search = search;

            return View(result);
        }

        [Authorize(Roles = "Librarian,Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Librarian,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PastPapersViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file.");
                return View(model);
            }

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var extension = Path.GetExtension(model.File.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("File", "Only PDF, DOC, and DOCX files are allowed.");
                return View(model);
            }

            var fileHash = GenerateFileHash(model.File);

            var duplicateExists = _context.PastPapers.Any(x => x.FileHash == fileHash);
            if (duplicateExists)
            {
                ModelState.AddModelError("File", "This file already exists in the system.");
                return View(model);
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/papers");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(model.File.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            var paper = new PastPapers
            {
                CourseCode = model.CourseCode,
                CourseTitle = model.CourseTitle,
                AcademicYear = model.AcademicYear,
                Semester = model.Semester,
                UploadedBy = User.Identity.Name ?? "System",
                FilePath = "/papers/" + uniqueFileName,
                Category = model.Category,
                OriginalFileName = model.File.FileName,
                FileHash = fileHash,
                UploadDate = DateTime.Now
            };

            _context.PastPapers.Add(paper);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Past paper uploaded successfully.";
            return RedirectToAction(nameof(PastPapers));
        }
        [Authorize(Roles = "Librarian,Admin")]
        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [Authorize(Roles = "Librarian,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(PastPaperZipImportViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.ZipFile == null || model.ZipFile.Length == 0)
            {
                ModelState.AddModelError("ZipFile", "Please select a ZIP file.");
                return View(model);
            }

            if (model.ExcelFile == null || model.ExcelFile.Length == 0)
            {
                ModelState.AddModelError("ExcelFile", "Please select an Excel file.");
                return View(model);
            }

            var zipExtension = Path.GetExtension(model.ZipFile.FileName).ToLowerInvariant();
            var excelExtension = Path.GetExtension(model.ExcelFile.FileName).ToLowerInvariant();

            if (zipExtension != ".zip")
            {
                ModelState.AddModelError("ZipFile", "Only .zip files are allowed.");
                return View(model);
            }

            if (excelExtension != ".xlsx")
            {
                ModelState.AddModelError("ExcelFile", "Only .xlsx files are allowed.");
                return View(model);
            }

            var tempRoot = Path.Combine(Path.GetTempPath(), "PastPaperImports", Guid.NewGuid().ToString());
            var extractedFolder = Path.Combine(tempRoot, "extracted");
            var zipPath = Path.Combine(tempRoot, Path.GetFileName(model.ZipFile.FileName));

            int importedCount = 0;
            int skippedCount = 0;
            var skippedReasons = new List<string>();

            try
            {
                Directory.CreateDirectory(tempRoot);
                Directory.CreateDirectory(extractedFolder);

                // Save ZIP temporarily
                using (var zipStream = new FileStream(zipPath, FileMode.Create))
                {
                    await model.ZipFile.CopyToAsync(zipStream);
                }

                // Extract ZIP
                ZipFile.ExtractToDirectory(zipPath, extractedFolder);

                // Check duplicate file names inside ZIP after extraction
                var duplicateZipNames = Directory
                    .GetFiles(extractedFolder, "*.*", SearchOption.AllDirectories)
                    .GroupBy(f => Path.GetFileName(f).Trim().ToLowerInvariant())
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateZipNames.Any())
                {
                    ModelState.AddModelError("", "The ZIP contains duplicate file names. Please rename them before importing.");
                    return View(model);
                }

                // Index extracted files by filename only
                var extractedFiles = Directory
                    .GetFiles(extractedFolder, "*.*", SearchOption.AllDirectories)
                    .ToDictionary(
                        f => Path.GetFileName(f).Trim().ToLowerInvariant(),
                        f => f
                    );



                using (var excelStream = new MemoryStream())
                {
                    await model.ExcelFile.CopyToAsync(excelStream);
                    excelStream.Position = 0;

                    using (var package = new ExcelPackage(excelStream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                        if (worksheet == null)
                        {
                            ModelState.AddModelError("", "The Excel file does not contain any worksheet.");
                            return View(model);
                        }

                        int rowCount = worksheet.Dimension?.Rows ?? 0;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var fileName = worksheet.Cells[row, 1].Text?.Trim();
                            var courseCode = worksheet.Cells[row, 2].Text?.Trim();
                            var courseTitle = worksheet.Cells[row, 3].Text?.Trim();
                            var academicYear = worksheet.Cells[row, 4].Text?.Trim();
                            var semester = worksheet.Cells[row, 5].Text?.Trim();
                            var categoryText = worksheet.Cells[row, 6].Text?.Trim();

                            if (string.IsNullOrWhiteSpace(fileName) ||
                                string.IsNullOrWhiteSpace(courseCode) ||
                                string.IsNullOrWhiteSpace(courseTitle) ||
                                string.IsNullOrWhiteSpace(academicYear) ||
                                string.IsNullOrWhiteSpace(semester) ||
                                string.IsNullOrWhiteSpace(categoryText))
                            {
                                skippedCount++;
                                skippedReasons.Add($"Row {row}: Missing required data.");
                                continue;
                            }

                            if (!Enum.TryParse<PastPaperCategory>(categoryText, true, out var parsedCategory))
                            {
                                skippedCount++;
                                skippedReasons.Add($"Row {row}: Invalid category '{categoryText}'.");
                                continue;
                            }

                            var normalizedFileName = Path.GetFileName(fileName).Trim().ToLowerInvariant();

                            if (!extractedFiles.TryGetValue(normalizedFileName, out var sourceFilePath))
                            {
                                skippedCount++;
                                skippedReasons.Add($"Row {row}: File '{fileName}' not found in ZIP.");
                                continue;
                            }

                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                            var fileExtension = Path.GetExtension(sourceFilePath).ToLowerInvariant();

                            if (!allowedExtensions.Contains(fileExtension))
                            {
                                skippedCount++;
                                skippedReasons.Add($"Row {row}: Unsupported file type '{fileExtension}'.");
                                continue;
                            }

                            var fileHash = GenerateFileHashFromPath(sourceFilePath);

                            var duplicateExists = await _context.PastPapers.AnyAsync(x => x.FileHash == fileHash);

                            if (duplicateExists)
                            {
                                skippedCount++;
                                skippedReasons.Add($"Row {row}: Duplicate file '{fileName}'.");
                                continue;
                            }

                            var finalUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/papers");
                            Directory.CreateDirectory(finalUploadsFolder);

                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(sourceFilePath);
                            var finalPath = Path.Combine(finalUploadsFolder, uniqueFileName);

                            System.IO.File.Copy(sourceFilePath, finalPath, true);

                            var paper = new PastPapers
                            {
                                CourseCode = courseCode,
                                CourseTitle = courseTitle,
                                AcademicYear = academicYear,
                                Semester = semester,
                                UploadedBy = User.Identity.Name ?? "System",
                                FilePath = "/papers/" + uniqueFileName,
                                Category = parsedCategory,
                                OriginalFileName = Path.GetFileName(sourceFilePath),
                                FileHash = fileHash,
                                UploadDate = DateTime.Now
                            };

                            _context.PastPapers.Add(paper);
                            importedCount++;
                        }
                    }
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = $"{importedCount} file(s) imported. {skippedCount} skipped.";
                TempData["SkippedReasons"] = string.Join(" | ", skippedReasons.Take(10));

                return RedirectToAction(nameof(PastPapers));
            }
            finally
            {
                if (Directory.Exists(tempRoot))
                {
                    Directory.Delete(tempRoot, true);
                }
            }
        }
        private string GenerateFileHash(IFormFile file)
        {
            using var sha256 = SHA256.Create();
            using var stream = file.OpenReadStream();

            var hashBytes = sha256.ComputeHash(stream);

            return BitConverter.ToString(hashBytes)
                .Replace("-", "")
                .ToLowerInvariant();
        }

        private string GenerateFileHashFromPath(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            var hashBytes = sha256.ComputeHash(stream);

            return BitConverter.ToString(hashBytes)
                .Replace("-", "")
                .ToLowerInvariant();
        }
    }
}