using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class LearningPathController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LearningPathController(
            LibraryDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<StudentProfile?> GetCurrentStudentAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;

            return await _context.StudentProfiles
                .FirstOrDefaultAsync(sp => sp.UserId == user.Id);
        }

        private async Task<StudentResourceProgress> GetOrCreateProgress(int studentId, int resourceId)
        {
            var progress = await _context.StudentResourceProgresses
                .FirstOrDefaultAsync(p => p.StudentProfileId == studentId && p.ResourceId == resourceId);

            if (progress == null)
            {
                progress = new StudentResourceProgress
                {
                    StudentProfileId = studentId,
                    ResourceId = resourceId
                };
                _context.StudentResourceProgresses.Add(progress);
                await _context.SaveChangesAsync();
            }
            return progress;
        }

        private void MarkCompleted(StudentResourceProgress progress)
        {
            if (!progress.IsCompleted)
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
            }
        }

        // ---------------------------
        // INDEX
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Index(int? semester)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return Forbid();

            var progressRecords = await _context.StudentResourceProgresses
                .Where(p => p.StudentProfileId == student.Id)
                .ToListAsync();

            var completedIds = progressRecords
                .Where(p => p.IsCompleted)
                .Select(p => p.ResourceId)
                .ToHashSet();

            var startedIds = progressRecords
                .Select(p => p.ResourceId)
                .ToHashSet();

            var now = DateTime.UtcNow;
            var studentProgram = student.Program?.Trim().ToLower();
            var studentLevel = student.Level?.Trim().ToLower();

            var standardsQuery = _context.CompetencyStandards
                .Where(c =>
                    c.Program.Trim().ToLower() == studentProgram &&
                    c.Level.Trim().ToLower() == studentLevel &&
                    c.IsActive &&
                    c.EffectiveFrom <= now &&
                    (c.EffectiveTo == null || c.EffectiveTo >= now));

            if (semester.HasValue)
                standardsQuery = standardsQuery.Where(c => c.Semester == semester.Value);

            var standards = await standardsQuery
                .Include(c => c.ResourceCompetencies)
                .ThenInclude(rc => rc.Resource)
                .ToListAsync();

            var units = standards
                .GroupBy(c => new { c.UnitCode, c.UnitName })
                .Select(group =>
                {
                    var resources = group
                        .SelectMany(c => c.ResourceCompetencies)
                        .Where(rc => rc.IsActive && rc.Resource != null && rc.Resource.IsActive)
                        .Select(rc => rc.Resource)
                        .GroupBy(r => r.Id)
                        .Select(g => g.First())
                        .ToList();

                    var total = resources.Count;
                    var completed = resources.Count(r => completedIds.Contains(r.Id));

                    return new UnitLearningPath
                    {
                        UnitCode = group.Key.UnitCode,
                        UnitName = group.Key.UnitName,
                        Books = resources.Where(r => r.Type == ResourceType.Book).ToList(),
                        PastPapers = resources.Where(r => r.Type == ResourceType.PastPaper).ToList(),
                        Videos = resources.Where(r => r.Type == ResourceType.YouTube).ToList(),
                        WebLinks = resources.Where(r => r.Type == ResourceType.WebLink).ToList(),
                        Documents = resources.Where(r => r.Type == ResourceType.Document).ToList(),
                        TotalResources = total,
                        CompletedResources = completed
                    };
                })
                .OrderBy(u => u.UnitCode)
                .ToList();

            return View(new LearningPathViewModel
            {
                Program = student.Program,
                Level = student.Level,
                Units = units,
                CompletedResourceIds = completedIds,
                StartedResourceIds = startedIds
            });
        }

        public async Task<IActionResult> DebugLearningPath()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return Unauthorized();

            var now = DateTime.UtcNow;

            var standards = await _context.CompetencyStandards
                .Include(c => c.ResourceCompetencies)
                .ThenInclude(rc => rc.Resource)
                .Where(c => c.Program == student.Program && c.Level == student.Level)
                .ToListAsync();

            var result = standards.Select(c => new
            {
                c.Id,
                c.Program,
                c.Level,
                c.UnitCode,
                c.UnitName,
                c.IsActive,
                c.EffectiveFrom,
                c.EffectiveTo,
                MatchesDate = c.EffectiveFrom <= now && (c.EffectiveTo == null || c.EffectiveTo >= now),
                ResourceMappings = c.ResourceCompetencies.Select(rc => new
                {
                    rc.ResourceId,
                    MappingIsActive = rc.IsActive,
                    ResourceTitle = rc.Resource != null ? rc.Resource.Title : null,
                    ResourceIsActive = rc.Resource != null && rc.Resource.IsActive
                })
            });

            return Json(result);
        }


        // VIDEO PROGRESS (80%)
        public class VideoProgressDto
        {
            public int ResourceId { get; set; }
            public double WatchedSeconds { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateVideoProgress([FromBody] VideoProgressDto dto)
        {
            if (dto == null || dto.ResourceId <= 0)
                return BadRequest();

            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Unauthorized();

            var resource = await _context.LibraryResources
                .FirstOrDefaultAsync(r => r.Id == dto.ResourceId && r.Type == ResourceType.YouTube);

            if (resource == null)
                return BadRequest();

            var progress = await GetOrCreateProgress(student.Id, dto.ResourceId);
            progress.WatchSeconds = Math.Max(progress.WatchSeconds, dto.WatchedSeconds);

            if (resource.DurationSeconds > 0)
            {
                var required = resource.DurationSeconds * 0.8;
                if (progress.WatchSeconds >= required)
                    MarkCompleted(progress);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> ViewWebLink(int id)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Unauthorized();

            var resource = await _context.LibraryResources
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && r.Type == ResourceType.WebLink && r.IsActive);

            if (resource == null)
                return NotFound();

            return View(new ViewWebLinkViewModel
            {
                ResourceId = resource.Id,
                Title = resource.Title,
                ExternalUrl = resource.UrlOrFilePath
            });
        }

        public class WebLinkQuizDto
        {
            public int ResourceId { get; set; }
            public int QuizScore { get; set; }
            public double TimeSpentSeconds { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitWebLinkQuiz([FromBody] WebLinkQuizDto dto)
        {
            if (dto == null || dto.ResourceId <= 0)
                return BadRequest();

            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Unauthorized();

            var resource = await _context.LibraryResources
                .FirstOrDefaultAsync(r => r.Id == dto.ResourceId &&
                                          r.Type == ResourceType.WebLink &&
                                          r.IsActive);

            if (resource == null)
                return BadRequest();

            var progress = await GetOrCreateProgress(student.Id, dto.ResourceId);

            progress.QuizScore = dto.QuizScore;
            progress.TimeSpentSeconds = Math.Max(progress.TimeSpentSeconds, dto.TimeSpentSeconds);

            if (progress.TimeSpentSeconds >= 120 && progress.QuizScore >= 50)
            {
                MarkCompleted(progress);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                completed = progress.IsCompleted,
                timeSpent = progress.TimeSpentSeconds,
                score = progress.QuizScore
            });
        }

        public class PastPaperFeedbackDto
        {
            public int ResourceId { get; set; }
            public int DifficultyRating { get; set; }
            public int ConfidenceRating { get; set; }
            public string ChallengingQuestions { get; set; }
            public string FeedbackNotes { get; set; }
        }
       

        public async Task<IActionResult> AttemptPastPaper(int id)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Unauthorized();

            var resource = await _context.LibraryResources
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id &&
                                          r.Type == ResourceType.PastPaper &&
                                          r.IsActive);

            if (resource == null)
                return NotFound();

            var vm = new AttemptPastPaperViewModel
            {
                ResourceId = resource.Id,
                Title = resource.Title,
                FilePath = resource.UrlOrFilePath
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPastPaperAttempt(AttemptPastPaperViewModel model)
        {
            if (!ModelState.IsValid)
                return View("AttemptPastPaper", model);

            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Unauthorized();

            var resource = await _context.LibraryResources
                .FirstOrDefaultAsync(r => r.Id == model.ResourceId &&
                                          r.Type == ResourceType.PastPaper &&
                                          r.IsActive);

            if (resource == null)
                return NotFound();

            var attempt = new PastPaperAttempt
            {
                ResourceId = model.ResourceId,
                StudentProfileId = student.Id,
                DifficultyRating = model.DifficultyRating,
                ConfidenceRating = model.ConfidenceRating,
                ChallengingQuestions = model.ChallengingQuestions,
                FeedbackNotes = model.FeedbackNotes,
                AttemptedAt = DateTime.UtcNow
            };

            _context.PastPaperAttempts.Add(attempt);

            var progress = await GetOrCreateProgress(student.Id, model.ResourceId);
            progress.AttemptCount += 1;

            if (progress.AttemptCount >= 1)
            {
                MarkCompleted(progress);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Past paper attempt submitted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PastPaperHistory()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Unauthorized();

            var attempts = await _context.PastPaperAttempts
                .Where(a => a.StudentProfileId == student.Id)
                .Join(_context.LibraryResources,
                      attempt => attempt.ResourceId,
                      resource => resource.Id,
                      (attempt, resource) => new PastPaperAttemptHistoryItemViewModel
                      {
                          Title = resource.Title,
                          DifficultyRating = attempt.DifficultyRating,
                          ConfidenceRating = attempt.ConfidenceRating,
                          ChallengingQuestions = attempt.ChallengingQuestions,
                          FeedbackNotes = attempt.FeedbackNotes,
                          AttemptedAt = attempt.AttemptedAt
                      })
                .OrderByDescending(a => a.AttemptedAt)
                .ToListAsync();

            var vm = new PastPaperAttemptHistoryViewModel
            {
                Attempts = attempts
            };

            return View(vm);
        }

       
        
        // WATCH VIDEO VIEW
        
        public async Task<IActionResult> WatchVideo(int id)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return Unauthorized();

            var resource = await _context.LibraryResources
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && r.Type == ResourceType.YouTube && r.IsActive);

            if (resource == null) return NotFound();

            var videoId = ExtractYouTubeId(resource.UrlOrFilePath);

            return View(new WatchVideoViewModel
            {
                ResourceId = resource.Id,
                Title = resource.Title,
                VideoId = videoId
            });
        }

        private string ExtractYouTubeId(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            if (url.Contains("youtu.be/"))
                return url.Split("youtu.be/")[1].Split('?')[0];

            if (url.Contains("v="))
                return url.Split("v=")[1].Split('&')[0];

            if (url.Contains("/embed/"))
                return url.Split("/embed/")[1].Split('?')[0];

            return null;
        }


        // DTO Class Definition
        public class DocumentProgressDto
        {
            public int ResourceId { get; set; }
            public double CoveragePercent { get; set; }
            public double TimeSpentSeconds { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDocumentProgress([FromBody] DocumentProgressDto dto)
        {
            if (dto == null || dto.ResourceId <= 0)
                return BadRequest();

            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Unauthorized();

            var resource = await _context.LibraryResources
                .FirstOrDefaultAsync(r => r.Id == dto.ResourceId &&
                                         (r.Type == ResourceType.Document || r.Type == ResourceType.Book) &&
                                         r.IsActive);

            if (resource == null)
                return BadRequest();

            var progress = await GetOrCreateProgress(student.Id, dto.ResourceId);

            progress.PageCoveragePercent = Math.Max(progress.PageCoveragePercent, dto.CoveragePercent);
            progress.TimeSpentSeconds = Math.Max(progress.TimeSpentSeconds, dto.TimeSpentSeconds);

            if (resource.Type == ResourceType.Document)
            {
                if (progress.PageCoveragePercent >= 80 && progress.TimeSpentSeconds >= 180)
                {
                    MarkCompleted(progress);
                }
            }
            else if (resource.Type == ResourceType.Book)
            {
                if (progress.PageCoveragePercent >= 80 && progress.TimeSpentSeconds >= 300)
                {
                    MarkCompleted(progress);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                completed = progress.IsCompleted,
                coverage = progress.PageCoveragePercent,
                timeSpent = progress.TimeSpentSeconds
            });
        }

        // Controller Method
        public async Task<IActionResult> ViewResource(int id)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Unauthorized();

            var resource = await _context.LibraryResources
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id &&
                                         (r.Type == ResourceType.Document || r.Type == ResourceType.Book) &&
                                         r.IsActive);

            if (resource == null)
                return NotFound();

            return View("ViewDocument", new ViewDocumentViewModel
            {
                ResourceId = resource.Id,
                Title = resource.Title,
                FilePath = resource.UrlOrFilePath
            });
        }
    }
}