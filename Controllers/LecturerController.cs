using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.IO;
using LibraryManagementSystem.Models.CDACC;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class LecturerController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LecturerController(
            LibraryDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<List<CompetencyStandard>> GetAssignedStandardsAsync(string lecturerUserId)
        {
            var assignments = await _context.LecturerUnitAssignments
                .Where(a => a.LecturerUserId == lecturerUserId && a.IsActive)
                .ToListAsync();

            var allActiveStandards = await _context.CompetencyStandards
                .Where(c => c.IsActive)
                .OrderBy(c => c.Program)
                .ThenBy(c => c.Level)
                .ThenBy(c => c.Semester)
                .ThenBy(c => c.UnitCode)
                .ToListAsync();

            var standards = allActiveStandards
                .Where(c => assignments.Any(a =>
                    a.Program == c.Program &&
                    a.Level == c.Level &&
                    a.Semester == c.Semester &&
                    a.UnitCode == c.UnitCode))
                .ToList();

            return standards;
        }

        private async Task<string> GenerateResourceReferenceNumber(ResourceType type)
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
        public async Task<IActionResult> Dashboard()
        {
            var lecturer = await _userManager.GetUserAsync(User);
            if (lecturer == null)
                return Unauthorized();

            var assignments = await _context.LecturerUnitAssignments
                .Where(a => a.LecturerUserId == lecturer.Id && a.IsActive)
                .ToListAsync();

            var programs = assignments.Select(a => a.Program).Distinct().ToList();
            var levels = assignments.Select(a => a.Level).Distinct().ToList();
            var semesters = assignments.Select(a => a.Semester).Distinct().ToList();
            var unitCodes = assignments.Select(a => a.UnitCode).Distinct().ToList();

            var standards = await _context.CompetencyStandards
                .Include(c => c.ResourceCompetencies)
                .ThenInclude(rc => rc.Resource)
                .Where(c =>
                    c.IsActive &&
                    programs.Contains(c.Program) &&
                    levels.Contains(c.Level) &&
                    semesters.Contains(c.Semester) &&
                    unitCodes.Contains(c.UnitCode))
                .ToListAsync();

            var unitSummaries = new List<LecturerUnitSummaryViewModel>();
            var studentsBehind = new List<LecturerStudentAlertViewModel>();
            var popularResources = new List<LecturerResourceSummaryViewModel>();
            var pastPaperInsights = new List<LecturerPastPaperInsightViewModel>();
            var resourceGapAlerts = new List<LecturerResourceGapAlertViewModel>();

            int totalResources = 0;
            int totalPastPapers = 0;

            var activeStudentIds = new HashSet<int>();

            foreach (var assignment in assignments)
            {
                var matchingStandards = standards
                    .Where(c =>
                        c.Program == assignment.Program &&
                        c.Level == assignment.Level &&
                        c.Semester == assignment.Semester &&
                        c.UnitCode == assignment.UnitCode)
                    .ToList();

                var resources = matchingStandards
                    .SelectMany(c => c.ResourceCompetencies)
                    .Where(rc => rc.IsActive && rc.Resource != null && rc.Resource.IsActive)
                    .Select(rc => rc.Resource)
                    .GroupBy(r => r.Id)
                    .Select(g => g.First())
                    .ToList();

                var resourceIds = resources.Select(r => r.Id).ToList();

                var booksCount = resources.Count(r => r.Type == ResourceType.Book);
                var documentsCount = resources.Count(r => r.Type == ResourceType.Document);
                var pastPapersCount = resources.Count(r => r.Type == ResourceType.PastPaper);
                var videosCount = resources.Count(r => r.Type == ResourceType.YouTube);
                var webLinksCount = resources.Count(r => r.Type == ResourceType.WebLink);
                var primaryStandardId = matchingStandards
                .Select(s => s.Id)
                .FirstOrDefault();

                var belowMinimumResources = resources.Count < 3;

                var gaps = new List<string>();

                if (resources.Count == 0) gaps.Add("No resources");
                if (booksCount == 0) gaps.Add("No books");
                if (documentsCount == 0) gaps.Add("No documents");
                if (pastPapersCount == 0) gaps.Add("No past papers");
                if (videosCount == 0) gaps.Add("No videos");
                if (belowMinimumResources) gaps.Add("Below minimum resources");

                if (gaps.Any())
                {
                    resourceGapAlerts.Add(new LecturerResourceGapAlertViewModel
                    {
                        CompetencyStandardId = primaryStandardId,
                        UnitCode = assignment.UnitCode,
                        UnitName = assignment.UnitName,
                        Program = assignment.Program,
                        Level = assignment.Level,
                        Semester = assignment.Semester,
                        TotalResourcesCount = resources.Count,
                        NoResources = resources.Count == 0,
                        NoBooks = booksCount == 0,
                        NoDocuments = documentsCount == 0,
                        NoPastPapers = pastPapersCount == 0,
                        NoVideos = videosCount == 0,
                        BelowMinimumResources = belowMinimumResources,
                        Summary = string.Join(", ", gaps)
                    });
                }
                totalResources += resources.Count;
                totalPastPapers += pastPapersCount;

                unitSummaries.Add(new LecturerUnitSummaryViewModel
                {
                    UnitCode = assignment.UnitCode,
                    UnitName = assignment.UnitName,
                    Program = assignment.Program,
                    Level = assignment.Level,
                    Semester = assignment.Semester,
                    BooksCount = booksCount,
                    DocumentsCount = documentsCount,
                    PastPapersCount = pastPapersCount,
                    VideosCount = videosCount,
                    WebLinksCount = webLinksCount,
                    TotalResourcesCount = resources.Count
                });

                if (resourceIds.Any())
                {
                    var studentProfiles = await _context.StudentProfiles
                        .Include(sp => sp.User)
                        .Where(sp => sp.Program == assignment.Program && sp.Level == assignment.Level)
                        .ToListAsync();

                    var studentIds = studentProfiles.Select(sp => sp.Id).ToList();

                    var progressRecords = await _context.StudentResourceProgresses
                        .Where(p => studentIds.Contains(p.StudentProfileId) && resourceIds.Contains(p.ResourceId))
                        .ToListAsync();

                    foreach (var studentId in progressRecords.Select(p => p.StudentProfileId).Distinct())
                    {
                        activeStudentIds.Add(studentId);
                    }

                    foreach (var student in studentProfiles)
                    {
                        int totalUnitResources = resourceIds.Count;
                        if (totalUnitResources == 0)
                            continue;

                        int completedResources = progressRecords
                            .Where(p => p.StudentProfileId == student.Id && p.IsCompleted)
                            .Select(p => p.ResourceId)
                            .Distinct()
                            .Count();

                        int completionPercentage = (int)Math.Round((double)completedResources * 100 / totalUnitResources);

                        if (completionPercentage < 50)
                        {
                            studentsBehind.Add(new LecturerStudentAlertViewModel
                            {
                                StudentName = student.User?.FullName ?? "Unknown",
                                AdmissionNumber = student.AdmissionNumber,
                                UnitCode = assignment.UnitCode,
                                UnitName = assignment.UnitName,
                                CompletedResources = completedResources,
                                TotalResources = totalUnitResources,
                                CompletionPercentage = completionPercentage
                            });
                        }
                    }

                    var resourceUsage = progressRecords
                        .GroupBy(p => p.ResourceId)
                        .Select(g => new
                        {
                            ResourceId = g.Key,
                            UsageCount = g.Count()
                        })
                        .ToList();

                    foreach (var usage in resourceUsage)
                    {
                        var resource = resources.FirstOrDefault(r => r.Id == usage.ResourceId);
                        if (resource != null)
                        {
                            popularResources.Add(new LecturerResourceSummaryViewModel
                            {
                                ResourceId = resource.Id,
                                Title = resource.Title,
                                UnitCode = assignment.UnitCode,
                                ResourceType = resource.Type.ToString(),
                                UsageCount = usage.UsageCount
                            });
                        }
                    }


                    var pastPaperResources = resources
                    .Where(r => r.Type == ResourceType.PastPaper)
                    .ToList();

                    if (pastPaperResources.Any())
                    {
                        var pastPaperIds = pastPaperResources.Select(r => r.Id).ToList();

                        var attempts = await _context.PastPaperAttempts
                            .Where(a => pastPaperIds.Contains(a.ResourceId))
                            .ToListAsync();

                        var groupedAttempts = attempts
                            .GroupBy(a => a.ResourceId)
                            .ToList();

                        foreach (var group in groupedAttempts)
                        {
                            var resource = pastPaperResources.FirstOrDefault(r => r.Id == group.Key);
                            if (resource != null)
                            {
                                pastPaperInsights.Add(new LecturerPastPaperInsightViewModel
                                {
                                    ResourceId = resource.Id,
                                    Title = resource.Title,
                                    UnitCode = assignment.UnitCode,
                                    AttemptCount = group.Count(),
                                    AverageDifficulty = group.Average(a => a.DifficultyRating),
                                    AverageConfidence = group.Average(a => a.ConfidenceRating)
                                });
                            }
                        }
                    }
                }
            }

            var mergedPopularResources = popularResources
                .GroupBy(r => new { r.ResourceId, r.Title, r.ResourceType })
                .Select(g => new LecturerResourceSummaryViewModel
                {
                    ResourceId = g.Key.ResourceId,
                    Title = g.Key.Title,
                    ResourceType = g.Key.ResourceType,
                    UnitCode = string.Join(", ", g.Select(x => x.UnitCode).Distinct()),
                    UsageCount = g.Sum(x => x.UsageCount)
                })
                .OrderByDescending(r => r.UsageCount)
                .Take(10)
                .ToList();

            var mergedPastPaperInsights = pastPaperInsights
         .GroupBy(p => new { p.ResourceId, p.Title })
         .Select(g => new LecturerPastPaperInsightViewModel
         {
             ResourceId = g.Key.ResourceId,
             Title = g.Key.Title,
             UnitCode = string.Join(", ", g.Select(x => x.UnitCode).Distinct()),
             AttemptCount = g.Sum(x => x.AttemptCount),
             AverageDifficulty = Math.Round(g.Average(x => x.AverageDifficulty), 2),
             AverageConfidence = Math.Round(g.Average(x => x.AverageConfidence), 2)
         })
                 .OrderByDescending(p => p.AverageDifficulty)
                 .ThenBy(p => p.AverageConfidence)
                 .Take(10)
                 .ToList();

            var vm = new LecturerDashboardViewModel
            {
                FirstName = lecturer.FirstName ?? lecturer.FullName ?? "Lecturer",
                AssignedUnitsCount = assignments.Count,
                TotalResourcesCount = totalResources,
                PastPapersCount = totalPastPapers,
                ActiveStudentsCount = activeStudentIds.Count,
                StudentsBehindCount = studentsBehind.Count,
                Units = unitSummaries.OrderBy(u => u.UnitCode).ToList(),
                StudentsBehind = studentsBehind
                   .OrderBy(s => s.CompletionPercentage)
                   .ThenBy(s => s.UnitCode)
                   .Take(10)
                   .ToList(),
                PopularResources = mergedPopularResources,
                PastPaperInsights = mergedPastPaperInsights,
                ResourceGapAlerts = resourceGapAlerts
                .OrderByDescending(g => g.NoResources)
                .ThenByDescending(g => g.BelowMinimumResources)
                .ThenBy(g => g.UnitCode)
                .ToList()
            };

            return View(vm);

          


        }


        [HttpGet]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> AddResource(int? competencyStandardId)
        {
            var lecturer = await _userManager.GetUserAsync(User);
            if (lecturer == null)
                return Unauthorized();

            var standards = await GetAssignedStandardsAsync(lecturer.Id);
            ViewBag.Standards = standards;

            var model = new LecturerAddResourceViewModel();

            if (competencyStandardId.HasValue)
            {
                var selectedStandard = standards.FirstOrDefault(s => s.Id == competencyStandardId.Value);
                if (selectedStandard != null)
                {
                    model.CompetencyStandardId = selectedStandard.Id;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> AddResource(LecturerAddResourceViewModel model)
        {
            var lecturer = await _userManager.GetUserAsync(User);
            if (lecturer == null)
                return Unauthorized();

            // reload assigned standards if the page must return
            ViewBag.Standards = await GetAssignedStandardsAsync(lecturer.Id);

            if (!ModelState.IsValid)
                return View(model);

            if (model.Type == null)
            {
                ModelState.AddModelError("Type", "Please select a resource type.");
                return View(model);
            }

            var assignments = await _context.LecturerUnitAssignments
                .Where(a => a.LecturerUserId == lecturer.Id && a.IsActive)
                .ToListAsync();

            var selectedStandard = await _context.CompetencyStandards
                .FirstOrDefaultAsync(c => c.Id == model.CompetencyStandardId && c.IsActive);

            if (selectedStandard == null)
            {
                ModelState.AddModelError("", "Selected unit is invalid.");
                return View(model);
            }

            bool isLecturerAssigned = assignments.Any(a =>
                a.Program == selectedStandard.Program &&
                a.Level == selectedStandard.Level &&
                a.Semester == selectedStandard.Semester &&
                a.UnitCode == selectedStandard.UnitCode);

            if (!isLecturerAssigned)
            {
                ModelState.AddModelError("", "You can only add resources to your assigned units.");
                return View(model);
            }

            string storedPath = null;
            string coverPath = null;

            var resourceType = model.Type.Value;

            // --------------------------------------------------
            // FILE BASED RESOURCES
            // --------------------------------------------------
            if (resourceType == ResourceType.Document ||
                resourceType == ResourceType.Book ||
                resourceType == ResourceType.PastPaper)
            {
                if (model.UploadedFile == null || model.UploadedFile.Length == 0)
                {
                    ModelState.AddModelError("UploadedFile", "A PDF file is required.");
                    return View(model);
                }

                var extension = Path.GetExtension(model.UploadedFile.FileName).ToLower();

                if (extension != ".pdf")
                {
                    ModelState.AddModelError("UploadedFile", "Only PDF files are allowed.");
                    return View(model);
                }

                // Book specific validation
                if (resourceType == ResourceType.Book)
                {
                    if (string.IsNullOrWhiteSpace(model.Author))
                    {
                        ModelState.AddModelError("Author", "Author is required for books.");
                        return View(model);
                    }

                    if (!string.IsNullOrWhiteSpace(model.ISBN))
                    {
                        var cleanIsbn = model.ISBN.Replace("-", "").Replace(" ", "");
                        if (!(cleanIsbn.Length == 10 || cleanIsbn.Length == 13) || !cleanIsbn.All(char.IsDigit))
                        {
                            ModelState.AddModelError("ISBN", "ISBN must be 10 or 13 digits.");
                            return View(model);
                        }
                    }
                }
                var folderName =
                    resourceType == ResourceType.Book ? "books" :
                    resourceType == ResourceType.PastPaper ? "pastpapers" :
                    "documents";

                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    folderName);

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + extension;
                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.UploadedFile.CopyToAsync(stream);
                }

                storedPath = $"/uploads/{folderName}/{fileName}";
            }

            
            // LINK BASED RESOURCES
            
            else if (resourceType == ResourceType.WebLink || resourceType == ResourceType.YouTube)
            {
                if (string.IsNullOrWhiteSpace(model.ExternalUrl))
                {
                    ModelState.AddModelError("ExternalUrl", "A valid URL is required.");
                    return View(model);
                }

                if (!Uri.TryCreate(model.ExternalUrl, UriKind.Absolute, out var uri))
                {
                    ModelState.AddModelError("ExternalUrl", "Invalid URL format.");
                    return View(model);
                }

                if (resourceType == ResourceType.YouTube &&
                    !uri.Host.Contains("youtube") &&
                    !uri.Host.Contains("youtu.be"))
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

            
            // OPTIONAL BOOK COVER
            
            if (resourceType == ResourceType.Book &&
                model.CoverImage != null &&
                model.CoverImage.Length > 0)
            {
                var coverExtension = Path.GetExtension(model.CoverImage.FileName).ToLower();

                if (coverExtension != ".jpg" &&
                    coverExtension != ".jpeg" &&
                    coverExtension != ".png")
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

            // --------------------------------------------------
            // CREATE RESOURCE
            // --------------------------------------------------

            var referenceNumber = await GenerateResourceReferenceNumber(resourceType);

            var resource = new LibraryResource
            {
                Title = model.Title,
                ReferenceNumber = referenceNumber,
                Type = resourceType,
                UrlOrFilePath = storedPath,
                Author = resourceType == ResourceType.Book ? model.Author : null,
                ISBN = resourceType == ResourceType.Book ? model.ISBN : null,
                CoverImagePath = resourceType == ResourceType.Book ? coverPath : null,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = lecturer.Id
            };

            _context.LibraryResources.Add(resource);
            await _context.SaveChangesAsync();

            var mapping = new ResourceCompetency
            {
                ResourceId = resource.Id,
                CompetencyStandardId = selectedStandard.Id,
                MappedByUserId = lecturer.Id,
                MappedAt = DateTime.Now,
                IsActive = true
            };

            _context.ResourceCompetencies.Add(mapping);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Resource added successfully.";

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> MyResources(string searchTerm, string unitCode, string resourceType, string status, bool myUploadsOnly = false)
        {
            var lecturer = await _userManager.GetUserAsync(User);
            if (lecturer == null)
                return Unauthorized();

            var assignments = await _context.LecturerUnitAssignments
                .Where(a => a.LecturerUserId == lecturer.Id && a.IsActive)
                .ToListAsync();

            var allMappings = await _context.ResourceCompetencies
                .Include(rc => rc.Resource)
                .Include(rc => rc.CompetencyStandard)
                .Where(rc => rc.Resource != null && rc.CompetencyStandard != null)
                .ToListAsync();

            var allLecturerResources = allMappings
                .Where(rc =>
                    assignments.Any(a =>
                        a.Program == rc.CompetencyStandard.Program &&
                        a.Level == rc.CompetencyStandard.Level &&
                        a.Semester == rc.CompetencyStandard.Semester &&
                        a.UnitCode == rc.CompetencyStandard.UnitCode))
                .Select(rc => new LecturerResourceListItemViewModel
                {
                    ResourceId = rc.ResourceId,
                    CompetencyStandardId = rc.CompetencyStandardId,
                    ReferenceNumber = rc.Resource.ReferenceNumber,
                    Title = rc.Resource.Title,
                    ResourceType = rc.Resource.Type.ToString(),
                    UnitCode = rc.CompetencyStandard.UnitCode,
                    UnitName = rc.CompetencyStandard.UnitName,
                    Program = rc.CompetencyStandard.Program,
                    Level = rc.CompetencyStandard.Level,
                    Semester = rc.CompetencyStandard.Semester,
                    IsActive = rc.IsActive && rc.Resource.IsActive,
                    UrlOrFilePath = rc.Resource.UrlOrFilePath,
                    CreatedByUserId = rc.Resource.CreatedByUserId
                })
                .ToList();

            var lecturerResources = allLecturerResources;

            if (myUploadsOnly)
            {
                lecturerResources = lecturerResources
                    .Where(r => r.CreatedByUserId == lecturer.Id)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                lecturerResources = lecturerResources
                    .Where(r =>
                        (!string.IsNullOrWhiteSpace(r.Title) && r.Title.ToLower().Contains(term)) ||
                        (!string.IsNullOrWhiteSpace(r.ReferenceNumber) && r.ReferenceNumber.ToLower().Contains(term)))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(unitCode))
            {
                lecturerResources = lecturerResources
                    .Where(r => r.UnitCode == unitCode)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(resourceType))
            {
                lecturerResources = lecturerResources
                    .Where(r => r.ResourceType == resourceType)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "Active")
                {
                    lecturerResources = lecturerResources
                        .Where(r => r.IsActive)
                        .ToList();
                }
                else if (status == "Inactive")
                {
                    lecturerResources = lecturerResources
                        .Where(r => !r.IsActive)
                        .ToList();
                }
            }

            var vm = new LecturerMyResourcesViewModel
            {
                SearchTerm = searchTerm,
                UnitCode = unitCode,
                ResourceType = resourceType,
                Status = status,
                MyUploadsOnly = myUploadsOnly,
                UnitCodes = allLecturerResources
                    .Select(r => r.UnitCode)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList(),
                ResourceTypes = allLecturerResources
                    .Select(r => r.ResourceType)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList(),
                Resources = lecturerResources
                    .OrderBy(r => r.UnitCode)
                    .ThenBy(r => r.Title)
                    .ToList()
            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> ReactivateResource(int resourceId, int competencyStandardId)
        {
            var lecturer = await _userManager.GetUserAsync(User);
            if (lecturer == null)
                return Unauthorized();

            var assignments = await _context.LecturerUnitAssignments
                .Where(a => a.LecturerUserId == lecturer.Id && a.IsActive)
                .ToListAsync();

            var mapping = await _context.ResourceCompetencies
                .Include(rc => rc.Resource)
                .Include(rc => rc.CompetencyStandard)
                .FirstOrDefaultAsync(rc =>
                    rc.ResourceId == resourceId &&
                    rc.CompetencyStandardId == competencyStandardId);

            if (mapping == null || mapping.Resource == null || mapping.CompetencyStandard == null)
                return NotFound();

            bool isLecturerAssigned = assignments.Any(a =>
                a.Program == mapping.CompetencyStandard.Program &&
                a.Level == mapping.CompetencyStandard.Level &&
                a.Semester == mapping.CompetencyStandard.Semester &&
                a.UnitCode == mapping.CompetencyStandard.UnitCode);

            if (!isLecturerAssigned)
                return Forbid();

            mapping.IsActive = true;

            // optional: if the resource itself had been marked inactive elsewhere,
            // restore it too so the mapping becomes usable again
            if (!mapping.Resource.IsActive)
            {
                mapping.Resource.IsActive = true;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Resource mapping reactivated successfully.";
            return RedirectToAction(nameof(MyResources));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> DeactivateResource(int resourceId, int competencyStandardId)
        {
            var lecturer = await _userManager.GetUserAsync(User);
            if (lecturer == null)
                return Unauthorized();

            var assignments = await _context.LecturerUnitAssignments
                .Where(a => a.LecturerUserId == lecturer.Id && a.IsActive)
                .ToListAsync();

            var mapping = await _context.ResourceCompetencies
                .Include(rc => rc.CompetencyStandard)
                .Include(rc => rc.Resource)
                .FirstOrDefaultAsync(rc =>
                    rc.ResourceId == resourceId &&
                    rc.CompetencyStandardId == competencyStandardId);

            if (mapping == null)
                return NotFound();

            bool isLecturerAssigned = assignments.Any(a =>
                a.Program == mapping.CompetencyStandard.Program &&
                a.Level == mapping.CompetencyStandard.Level &&
                a.Semester == mapping.CompetencyStandard.Semester &&
                a.UnitCode == mapping.CompetencyStandard.UnitCode);

            if (!isLecturerAssigned)
                return Forbid();

            mapping.IsActive = false;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Resource mapping deactivated successfully.";
            return RedirectToAction(nameof(MyResources));
        }

        [HttpGet]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> EditResource(int resourceId, int competencyStandardId)
        {
            var lecturer = await _userManager.GetUserAsync(User);
            if (lecturer == null)
                return Unauthorized();

            var assignments = await _context.LecturerUnitAssignments
                .Where(a => a.LecturerUserId == lecturer.Id && a.IsActive)
                .ToListAsync();

            var mapping = await _context.ResourceCompetencies
                .Include(rc => rc.Resource)
                .Include(rc => rc.CompetencyStandard)
                .FirstOrDefaultAsync(rc =>
                    rc.ResourceId == resourceId &&
                    rc.CompetencyStandardId == competencyStandardId &&
                    rc.IsActive);

            if (mapping == null || mapping.Resource == null || mapping.CompetencyStandard == null)
                return NotFound();

            bool isLecturerAssigned = assignments.Any(a =>
                a.Program == mapping.CompetencyStandard.Program &&
                a.Level == mapping.CompetencyStandard.Level &&
                a.Semester == mapping.CompetencyStandard.Semester &&
                a.UnitCode == mapping.CompetencyStandard.UnitCode);

            if (!isLecturerAssigned)
                return Forbid();

            ViewBag.Standards = await GetAssignedStandardsAsync(lecturer.Id);

            var vm = new LecturerAddResourceViewModel
            {
                CompetencyStandardId = mapping.CompetencyStandardId,
                Title = mapping.Resource.Title,
                Type = mapping.Resource.Type,
                Author = mapping.Resource.Author,
                ISBN = mapping.Resource.ISBN,
                ExternalUrl = mapping.Resource.Type == ResourceType.WebLink || mapping.Resource.Type == ResourceType.YouTube
                    ? mapping.Resource.UrlOrFilePath
                    : null,
                IsActive = mapping.Resource.IsActive
            };

            ViewBag.ResourceId = mapping.ResourceId;
            ViewBag.ExistingFilePath = mapping.Resource.UrlOrFilePath;
            ViewBag.ExistingCoverPath = mapping.Resource.CoverImagePath;
            ViewBag.MappingStandardId = mapping.CompetencyStandardId;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> EditResource(int resourceId, int competencyStandardId, LecturerAddResourceViewModel model)
        {
            var lecturer = await _userManager.GetUserAsync(User);
            if (lecturer == null)
                return Unauthorized();

            var assignments = await _context.LecturerUnitAssignments
                .Where(a => a.LecturerUserId == lecturer.Id && a.IsActive)
                .ToListAsync();

            ViewBag.Standards = await GetAssignedStandardsAsync(lecturer.Id);
            ViewBag.ResourceId = resourceId;
            ViewBag.MappingStandardId = competencyStandardId;

            var mapping = await _context.ResourceCompetencies
                .Include(rc => rc.Resource)
                .Include(rc => rc.CompetencyStandard)
                .FirstOrDefaultAsync(rc =>
                    rc.ResourceId == resourceId &&
                    rc.CompetencyStandardId == competencyStandardId &&
                    rc.IsActive);

            if (mapping == null || mapping.Resource == null || mapping.CompetencyStandard == null)
                return NotFound();

            bool isLecturerAssigned = assignments.Any(a =>
                a.Program == mapping.CompetencyStandard.Program &&
                a.Level == mapping.CompetencyStandard.Level &&
                a.Semester == mapping.CompetencyStandard.Semester &&
                a.UnitCode == mapping.CompetencyStandard.UnitCode);

            if (!isLecturerAssigned)
                return Forbid();

            if (!ModelState.IsValid)
            {
                ViewBag.ExistingFilePath = mapping.Resource.UrlOrFilePath;
                ViewBag.ExistingCoverPath = mapping.Resource.CoverImagePath;
                return View(model);
            }

            if (model.Type == null)
            {
                ModelState.AddModelError("Type", "Please select a resource type.");
                ViewBag.ExistingFilePath = mapping.Resource.UrlOrFilePath;
                ViewBag.ExistingCoverPath = mapping.Resource.CoverImagePath;
                return View(model);
            }

            var selectedStandard = await _context.CompetencyStandards
                .FirstOrDefaultAsync(c => c.Id == model.CompetencyStandardId && c.IsActive);

            if (selectedStandard == null)
            {
                ModelState.AddModelError("", "Selected unit is invalid.");
                ViewBag.ExistingFilePath = mapping.Resource.UrlOrFilePath;
                ViewBag.ExistingCoverPath = mapping.Resource.CoverImagePath;
                return View(model);
            }

            bool isNewStandardAssigned = assignments.Any(a =>
                a.Program == selectedStandard.Program &&
                a.Level == selectedStandard.Level &&
                a.Semester == selectedStandard.Semester &&
                a.UnitCode == selectedStandard.UnitCode);

            if (!isNewStandardAssigned)
            {
                ModelState.AddModelError("", "You can only assign resources to your own units.");
                ViewBag.ExistingFilePath = mapping.Resource.UrlOrFilePath;
                ViewBag.ExistingCoverPath = mapping.Resource.CoverImagePath;
                return View(model);
            }

            string storedPath = mapping.Resource.UrlOrFilePath;
            string coverPath = mapping.Resource.CoverImagePath;

            var resourceType = model.Type.Value;

            // FILE-BASED RESOURCES
            if (resourceType == ResourceType.Document ||
                resourceType == ResourceType.Book ||
                resourceType == ResourceType.PastPaper)
            {
                if (resourceType == ResourceType.Book)
                {
                    if (string.IsNullOrWhiteSpace(model.Author))
                    {
                        ModelState.AddModelError("Author", "Author is required for books.");
                        ViewBag.ExistingFilePath = mapping.Resource.UrlOrFilePath;
                        ViewBag.ExistingCoverPath = mapping.Resource.CoverImagePath;
                        return View(model);
                    }

                    if (!string.IsNullOrWhiteSpace(model.ISBN))
                    {
                        var cleanIsbn = model.ISBN.Replace("-", "").Replace(" ", "");
                        if (!(cleanIsbn.Length == 10 || cleanIsbn.Length == 13) || !cleanIsbn.All(char.IsDigit))
                        {
                            ModelState.AddModelError("ISBN", "ISBN must be 10 or 13 digits.");
                            ViewBag.ExistingFilePath = mapping.Resource.UrlOrFilePath;
                            ViewBag.ExistingCoverPath = mapping.Resource.CoverImagePath;
                            return View(model);
                        }
                    }
                }

                bool existingLooksLikePdf = !string.IsNullOrWhiteSpace(mapping.Resource.UrlOrFilePath) &&
                                            mapping.Resource.UrlOrFilePath.ToLower().EndsWith(".pdf");

                if ((model.UploadedFile == null || model.UploadedFile.Length == 0) && !existingLooksLikePdf)
                {
                    ModelState.AddModelError("UploadedFile", "A PDF file is required for this resource type.");
                    ViewBag.ExistingFilePath = mapping.Resource.UrlOrFilePath;
                    ViewBag.ExistingCoverPath = mapping.Resource.CoverImagePath;
                    return View(model);
                }

                if (model.UploadedFile != null && model.UploadedFile.Length > 0)
                {
                    var extension = Path.GetExtension(model.UploadedFile.FileName).ToLower();
                    if (extension != ".pdf")
                    {
                        ModelState.AddModelError("UploadedFile", "Only PDF files are allowed.");
                        ViewBag.ExistingFilePath = mapping.Resource.UrlOrFilePath;
                        ViewBag.ExistingCoverPath = mapping.Resource.CoverImagePath;
                        return View(model);
                    }

                    var folderName =
                        resourceType == ResourceType.Book ? "books" :
                        resourceType == ResourceType.PastPaper ? "pastpapers" :
                        "documents";

                    var uploadsFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        folderName);

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid() + extension;
                    var fullPath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await model.UploadedFile.CopyToAsync(stream);
                    }

                    storedPath = $"/uploads/{folderName}/{fileName}";
                }
            }
            // LINK-BASED RESOURCES
            else if (resourceType == ResourceType.WebLink || resourceType == ResourceType.YouTube)
            {
                if (string.IsNullOrWhiteSpace(model.ExternalUrl))
                {
                    ModelState.AddModelError("ExternalUrl", "A valid URL is required.");
                    ViewBag.ExistingFilePath = mapping.Resource.UrlOrFilePath;
                    ViewBag.ExistingCoverPath = mapping.Resource.CoverImagePath;
                    return View(model);
                }

                if (!Uri.TryCreate(model.ExternalUrl, UriKind.Absolute, out var uri))
                {
                    ModelState.AddModelError("ExternalUrl", "Invalid URL format.");
                    ViewBag.ExistingFilePath = mapping.Resource.UrlOrFilePath;
                    ViewBag.ExistingCoverPath = mapping.Resource.CoverImagePath;
                    return View(model);
                }

                if (resourceType == ResourceType.YouTube &&
                    !uri.Host.Contains("youtube") &&
                    !uri.Host.Contains("youtu.be"))
                {
                    ModelState.AddModelError("ExternalUrl", "URL must be a valid YouTube link.");
                    ViewBag.ExistingFilePath = mapping.Resource.UrlOrFilePath;
                    ViewBag.ExistingCoverPath = mapping.Resource.CoverImagePath;
                    return View(model);
                }

                storedPath = model.ExternalUrl.Trim();
            }
            else
            {
                ModelState.AddModelError("", "Invalid resource type.");
                ViewBag.ExistingFilePath = mapping.Resource.UrlOrFilePath;
                ViewBag.ExistingCoverPath = mapping.Resource.CoverImagePath;
                return View(model);
            }

            // OPTIONAL BOOK COVER
            if (resourceType == ResourceType.Book && model.CoverImage != null && model.CoverImage.Length > 0)
            {
                var coverExtension = Path.GetExtension(model.CoverImage.FileName).ToLower();
                if (coverExtension != ".jpg" && coverExtension != ".jpeg" && coverExtension != ".png")
                {
                    ModelState.AddModelError("CoverImage", "Cover image must be JPG or PNG.");
                    ViewBag.ExistingFilePath = mapping.Resource.UrlOrFilePath;
                    ViewBag.ExistingCoverPath = mapping.Resource.CoverImagePath;
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

            mapping.Resource.Title = model.Title;
            mapping.Resource.Type = resourceType;
            mapping.Resource.UrlOrFilePath = storedPath;
            mapping.Resource.Author = resourceType == ResourceType.Book ? model.Author : null;
            mapping.Resource.ISBN = resourceType == ResourceType.Book ? model.ISBN : null;
            mapping.Resource.CoverImagePath = resourceType == ResourceType.Book ? coverPath : null;
            mapping.Resource.IsActive = model.IsActive;

            // Move mapping to another lecturer-assigned standard if changed
            mapping.CompetencyStandardId = model.CompetencyStandardId;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Resource updated successfully.";
            return RedirectToAction(nameof(MyResources));
        }

        [HttpGet]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> OpenResource(int resourceId, int competencyStandardId)
        {
            var lecturer = await _userManager.GetUserAsync(User);
            if (lecturer == null)
                return Unauthorized();

            var assignments = await _context.LecturerUnitAssignments
                .Where(a => a.LecturerUserId == lecturer.Id && a.IsActive)
                .ToListAsync();

            var mapping = await _context.ResourceCompetencies
                .Include(rc => rc.Resource)
                .Include(rc => rc.CompetencyStandard)
                .FirstOrDefaultAsync(rc =>
                    rc.ResourceId == resourceId &&
                    rc.CompetencyStandardId == competencyStandardId &&
                    rc.IsActive);

            if (mapping == null || mapping.Resource == null || mapping.CompetencyStandard == null)
                return NotFound();

            bool isLecturerAssigned = assignments.Any(a =>
                a.Program == mapping.CompetencyStandard.Program &&
                a.Level == mapping.CompetencyStandard.Level &&
                a.Semester == mapping.CompetencyStandard.Semester &&
                a.UnitCode == mapping.CompetencyStandard.UnitCode);

            if (!isLecturerAssigned)
                return Forbid();

            if (!mapping.Resource.IsActive || string.IsNullOrWhiteSpace(mapping.Resource.UrlOrFilePath))
                return NotFound();

            return Redirect(mapping.Resource.UrlOrFilePath);
        }
    }
}