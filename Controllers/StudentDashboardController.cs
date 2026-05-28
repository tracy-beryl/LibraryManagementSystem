using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Service;
using System.Security.Claims;

namespace LibraryManagementSystem.Controllers
{
    public class StudentDashboardController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRecommendationService _recommendationService;

      
        public StudentDashboardController(
            LibraryDbContext context,
            UserManager<ApplicationUser> userManager, IRecommendationService recommendationService)
        {
            _context = context;
            _userManager = userManager;
            _recommendationService = recommendationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var now = DateTime.UtcNow;
            var firstName = !string.IsNullOrWhiteSpace(currentUser?.FirstName)
            ? currentUser.FirstName
           : currentUser?.FullName?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "User";

            var myBorrowedBooks = await _context.Loans
                .CountAsync(l => l.UserId == currentUser.Id && l.ReturnDate == null);

            var myReturnedBooks = await _context.Loans
                .CountAsync(l => l.UserId == currentUser.Id && l.ReturnDate != null);

            var dueSoonBooks = await _context.Loans
                .Where(l => l.UserId == currentUser.Id
                            && l.ReturnDate == null
                            && l.DueDate >= now
                            && l.DueDate <= now.AddDays(3))
                .Include(l => l.Book)
                .OrderBy(l => l.DueDate)
                .Select(l => new StudentLoanItemViewModel
                {
                    LoanId = l.Id,
                    BookId = l.BookId,
                    Title = l.Book.Title,
                    Author = l.Book.Author,
                    PhotoPath = l.Book.PhotoPath,
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    DaysLeft = EF.Functions.DateDiffDay(now, l.DueDate)
                })
                .ToListAsync();

            var overdueBooks = await _context.Loans
                .Where(l => l.UserId == currentUser.Id
                            && l.ReturnDate == null
                            && l.DueDate < now)
                .Include(l => l.Book)
                .OrderBy(l => l.DueDate)
                .Select(l => new StudentLoanItemViewModel
                {
                    LoanId = l.Id,
                    BookId = l.BookId,
                    Title = l.Book.Title,
                    Author = l.Book.Author,
                    PhotoPath = l.Book.PhotoPath,
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    DaysLeft = EF.Functions.DateDiffDay(now, l.DueDate)
                })
                .ToListAsync();

            var currentLoans = await _context.Loans
                .Where(l => l.UserId == currentUser.Id && l.ReturnDate == null)
                .Include(l => l.Book)
                .OrderBy(l => l.DueDate)
                .Select(l => new StudentLoanItemViewModel
                {
                    LoanId = l.Id,
                    BookId = l.BookId,
                    Title = l.Book.Title,
                    Author = l.Book.Author,
                    PhotoPath = l.Book.PhotoPath,
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    DaysLeft = EF.Functions.DateDiffDay(now, l.DueDate)
                })
                .Take(5)
                .ToListAsync();

            // PERSONAL top 5 borrowed books
            var topBorrowedBooks = await _context.Loans
                .Include(l => l.Book)
                .Where(l => l.UserId == currentUser.Id && l.Book != null)
                .GroupBy(l => new
                {
                    l.BookId,
                    l.Book.Title,
                    l.Book.Author,
                    l.Book.PhotoPath,
                    l.Book.Category
                })
                .Select(g => new TopBorrowedBookViewModel
                {
                    BookId = g.Key.BookId,
                    Title = g.Key.Title,
                    Author = g.Key.Author,
                    PhotoPath = g.Key.PhotoPath,
                    Category = g.Key.Category,
                    BorrowCount = g.Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .ThenBy(x => x.Title)
                .Take(5)
                .ToListAsync();

            var activeProjects = await _context.ProjectMembers
                .Where(pm => pm.StudentId == currentUser.Id)
                .Include(pm => pm.Project)
                .ThenInclude(p => p.ProjectMembers)
                .Include(pm => pm.Project)
                .ThenInclude(p => p.ProjectResources)
                .Include(pm => pm.Project)
                .ThenInclude(p => p.ProjectDeadlines)
                .Include(pm => pm.Project)
                .ThenInclude(p => p.ProjectMessages)
                .Select(pm => new StudentProjectItemViewModel
                {
                    ProjectId = pm.ProjectId,
                    ProjectName = pm.Project.ProjectName,
                    Description = pm.Project.Description,
                    Role = pm.Role,
                    TeamMembers = pm.Project.ProjectMembers.Count(),
                    ResourceCount = pm.Project.ProjectResources.Count(),
                    DeadlineCount = pm.Project.ProjectDeadlines.Count(),
                    UnreadMessages = pm.Project.ProjectMessages.Count(m => m.SentAt > pm.LastReadAt && m.SenderId != currentUser.Id),
                    CreatedAt = pm.Project.CreatedAt
                })
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();

            var pendingInvitations = await _context.ProjectInvitations
                .CountAsync(i => i.StudentId == currentUser.Id && i.Status == "Pending");

            var unreadProjectMessages = await _context.ProjectMembers
                .Where(pm => pm.StudentId == currentUser.Id)
                .SelectMany(pm => pm.Project.ProjectMessages
                    .Where(m => m.SentAt > pm.LastReadAt && m.SenderId != currentUser.Id))
                .CountAsync();

            var upcomingDeadlines = await _context.ProjectMembers
                .Where(pm => pm.StudentId == currentUser.Id)
                .SelectMany(pm => pm.Project.ProjectDeadlines.Select(d => new StudentProjectDeadlineViewModel
                {
                    ProjectId = pm.ProjectId,
                    ProjectName = pm.Project.ProjectName,
                    DeadlineTitle = d.Title,
                    DueDate = d.DueDate,
                    DaysLeft = EF.Functions.DateDiffDay(now, d.DueDate)
                }))
                .Where(d => d.DueDate >= now)
                .OrderBy(d => d.DueDate)
                .Take(5)
                .ToListAsync();

            // Learning Path summary
            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(sp => sp.UserId == currentUser.Id);

            int learningResourcesTotal = 0;
            int learningResourcesCompleted = 0;
            double learningCompletionPercent = 0;
            var unitProgress = new System.Collections.Generic.List<LearningUnitProgressItemViewModel>();
            var continueLearning = new System.Collections.Generic.List<LearningContinueItemViewModel>();

            if (studentProfile != null)
            {
                var studentProgram = studentProfile.Program?.Trim().ToLower();
                var studentLevel = studentProfile.Level?.Trim().ToLower();

                var standards = await _context.CompetencyStandards
                    .Where(c =>
                        c.Program.Trim().ToLower() == studentProgram &&
                        c.Level.Trim().ToLower() == studentLevel &&
                        c.IsActive &&
                        c.EffectiveFrom <= now &&
                        (c.EffectiveTo == null || c.EffectiveTo >= now))
                    .Include(c => c.ResourceCompetencies)
                    .ThenInclude(rc => rc.Resource)
                    .ToListAsync();

                var progressRecords = await _context.StudentResourceProgresses
                    .Where(p => p.StudentProfileId == studentProfile.Id)
                    .ToListAsync();

                var completedIds = progressRecords
                    .Where(p => p.IsCompleted)
                    .Select(p => p.ResourceId)
                    .ToHashSet();

                var startedIds = progressRecords
                    .Select(p => p.ResourceId)
                    .ToHashSet();

                var groupedUnits = standards
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

                        return new
                        {
                            group.Key.UnitCode,
                            group.Key.UnitName,
                            Resources = resources,
                            Total = total,
                            Completed = completed
                        };
                    })
                    .OrderBy(x => x.UnitCode)
                    .ToList();

                learningResourcesTotal = groupedUnits.Sum(x => x.Total);
                learningResourcesCompleted = groupedUnits.Sum(x => x.Completed);
                learningCompletionPercent = learningResourcesTotal == 0
                    ? 0
                    : Math.Round((double)learningResourcesCompleted / learningResourcesTotal * 100, 1);

                unitProgress = groupedUnits
                    .Where(x => x.Total > 0)
                    .Select(x => new LearningUnitProgressItemViewModel
                    {
                        UnitCode = x.UnitCode,
                        UnitName = x.UnitName,
                        TotalResources = x.Total,
                        CompletedResources = x.Completed
                    })
                    .OrderBy(x => ((double)x.CompletedResources / Math.Max(x.TotalResources, 1)))
                    .ThenBy(x => x.UnitCode)
                    .Take(5)
                    .ToList();

                continueLearning = groupedUnits
                    .SelectMany(x => x.Resources.Select(r => new LearningContinueItemViewModel
                    {
                        ResourceId = r.Id,
                        Title = r.Title,
                        ResourceType = r.Type.ToString(),
                        UnitCode = x.UnitCode,
                        UnitName = x.UnitName
                    }))
                    .Where(r => startedIds.Contains(r.ResourceId) && !completedIds.Contains(r.ResourceId))
                    .Take(5)
                    .ToList();

                if (!continueLearning.Any())
                {
                    continueLearning = groupedUnits
                        .SelectMany(x => x.Resources.Select(r => new LearningContinueItemViewModel
                        {
                            ResourceId = r.Id,
                            Title = r.Title,
                            ResourceType = r.Type.ToString(),
                            UnitCode = x.UnitCode,
                            UnitName = x.UnitName
                        }))
                        .Where(r => !completedIds.Contains(r.ResourceId))
                        .Take(5)
                        .ToList();
                }
            }

            var recommendedBooks = await _recommendationService
            .GetRecommendedBooksForUserAsync(currentUser.Id, 6);

            var viewModel = new StudentDashboardViewModel
            {
                FirstName = firstName,
                MyBorrowedBooks = myBorrowedBooks,
                MyHistory = myReturnedBooks,
                DueSoonCount = dueSoonBooks.Count,
                OverdueCount = overdueBooks.Count,
                CurrentLoans = currentLoans,
                DueSoonBooks = dueSoonBooks,
                OverdueBooks = overdueBooks,
                TopBorrowedBooks = topBorrowedBooks,
                MyActiveProjects = activeProjects.Count,
                PendingInvitations = pendingInvitations,
                UnreadProjectMessages = unreadProjectMessages,
                ActiveProjects = activeProjects,
                UpcomingDeadlines = upcomingDeadlines,
                LearningResourcesTotal = learningResourcesTotal,
                LearningResourcesCompleted = learningResourcesCompleted,
                LearningCompletionPercent = learningCompletionPercent,
                UnitProgress = unitProgress,
                ContinueLearning = continueLearning,
                RecommendedBooks = recommendedBooks
            };

            return View(viewModel);

           
        }
    }
}