using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using LibraryManagementSystem.Filters;

namespace LibraryManagementSystem.Controllers
{
    [StudentOnly]
    public class StudyHubController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<CollaborationHub> _hub;

        public StudyHubController(
            LibraryDbContext context,
            UserManager<ApplicationUser> userManager,
            IHubContext<CollaborationHub> hub)
        {
            _context = context;
            _userManager = userManager;
            _hub = hub;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<ApplicationUser?> GetCurrentStudentAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return null;

            return await _context.Users
                .Include(u => u.StudentProfile)
                .FirstOrDefaultAsync(u =>
                    u.Id == userId &&
                    u.StudentProfile != null);
        }

        [Authorize(Roles = "Student")]

        public async Task<IActionResult> Index()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Forbid();

            var projects = await _context.ProjectMembers
                .Where(pm => pm.StudentId == student.Id)
                .Select(pm => pm.Project)
                .ToListAsync();

            return View(projects);
        }
        private async Task<ProjectMember?> GetMembership(int projectId)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return null;

            return await _context.ProjectMembers
                .FirstOrDefaultAsync(pm =>
                    pm.ProjectId == projectId &&
                    pm.StudentId == student.Id);
        }

        public async Task<IActionResult> Project(int id)
        {
            var membership = await GetMembership(id);

            if (membership == null)
                return Forbid();

            membership.LastReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var project = await _context.StudyProjects
                .Where(p => p.Id == id)
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.Student)
                .Include(p => p.ProjectResources)
                    .ThenInclude(r => r.Views)
                .Include(p => p.ProjectDeadlines)
                .Include(p => p.ProjectMessages)
                    .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync();

            if (project == null)
                return NotFound();

            var userId = GetUserId();

            var unreadCount = project.ProjectMessages
                .Count(m =>
                    m.SenderId != userId &&
                    m.SentAt > membership.LastReadAt);

            var viewModel = new ProjectDetailsViewModel
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                Description = project.Description,
                IsOwner = project.OwnerId == userId,
                UnreadCount = unreadCount,

                Members = project.ProjectMembers
                    .Select(pm => new ProjectMember
                    {
                        ProjectId = pm.ProjectId,
                        StudentId = pm.StudentId,
                        Role = pm.Role,
                        Student = pm.Student
                    }).ToList(),

                Resources = project.ProjectResources
                    .Select(r => new ResourceViewModel
                    {
                        Id = r.Id,
                        Title = r.Title,
                        Type = r.Type,
                        FilePath = r.FilePath,
                        Url = r.Url,
                        ViewCount = r.Views.Count
                    }).ToList(),

                Deadlines = project.ProjectDeadlines.ToList(),

                Messages = project.ProjectMessages
                    .OrderBy(m => m.SentAt)
                    .Select(m => new MessageViewModel
                    {
                        SenderId = m.SenderId,
                        SenderName = m.Sender.FullName,
                        Text = m.Message,
                        SentAt = m.SentAt
                    }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Create()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Forbid();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectViewModel model)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Forbid();

            if (!ModelState.IsValid)
                return View(model);

            var project = new StudyProject
            {
                ProjectName = model.ProjectName,
                Description = model.Description,
                OwnerId = student.Id
            };

            _context.StudyProjects.Add(project);
            await _context.SaveChangesAsync();

            _context.ProjectMembers.Add(new ProjectMember
            {
                ProjectId = project.Id,
                StudentId = student.Id,
                Role = "Owner",
                LastReadAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Project", new { id = project.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int projectId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return BadRequest();

            var membership = await GetMembership(projectId);
            if (membership == null)
                return Forbid();

            var user = await _userManager.GetUserAsync(User);

            var msg = new ProjectMessage
            {
                ProjectId = projectId,
                SenderId = user.Id,
                Message = message.Trim(),
                SentAt = DateTime.UtcNow
            };

            _context.ProjectMessages.Add(msg);
            await _context.SaveChangesAsync();

            await _hub.Clients.Group($"Project-{projectId}")
                .SendAsync("ReceiveMessage",
                    user.Id,
                    user.FullName,
                    msg.Message,
                    msg.SentAt);

            return Ok();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InviteMember(int projectId, string emailOrAdmission)
        {
            var userId = GetUserId();

            var isOwner = await _context.StudyProjects
                .AnyAsync(p => p.Id == projectId && p.OwnerId == userId);

            if (!isOwner)
                return Forbid();

            var studentProfile = await _context.StudentProfiles
                .Include(sp => sp.User)
                .FirstOrDefaultAsync(sp =>
                    sp.AdmissionNumber == emailOrAdmission ||
                    sp.User.Email == emailOrAdmission);

            if (studentProfile == null)
                return BadRequest("Student not found");

            var student = studentProfile.User;

            var alreadyInvited = await _context.ProjectInvitations
                .AnyAsync(i => i.ProjectId == projectId && i.StudentId == student.Id && i.Status == "Pending");

            if (alreadyInvited)
                return RedirectToAction("Project", new { id = projectId });

            var invitation = new ProjectInvitation
            {
                ProjectId = projectId,
                StudentId = student.Id,
                Status = "Pending"
            };

            _context.ProjectInvitations.Add(invitation);
            await _context.SaveChangesAsync();

            return RedirectToAction("Project", new { id = projectId });
        }
        public async Task<IActionResult> MyInvitations()
        {
            var userId = GetUserId();

            var invitations = await _context.ProjectInvitations
                .Include(i => i.Project)
                    .ThenInclude(p => p.Owner)
                .Where(i => i.StudentId == userId && i.Status == "Pending")
                .ToListAsync();

            return View(invitations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptInvitation(int id)
        {
            var userId = GetUserId();

            var invite = await _context.ProjectInvitations
                .FirstOrDefaultAsync(i => i.Id == id && i.StudentId == userId);

            if (invite == null)
                return NotFound();

            invite.Status = "Accepted";

            _context.ProjectMembers.Add(new ProjectMember
            {
                ProjectId = invite.ProjectId,
                StudentId = userId,
                Role = "Member"
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("MyInvitations");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectInvitation(int id)
        {
            var userId = GetUserId();

            var invite = await _context.ProjectInvitations
                .FirstOrDefaultAsync(i => i.Id == id && i.StudentId == userId);

            if (invite == null)
                return NotFound();

            invite.Status = "Rejected";

            await _context.SaveChangesAsync();

            return RedirectToAction("MyInvitations");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddResource(int projectId,
                                                     string title,
                                                     ResourceType type,
                                                     IFormFile file,
                                                     string url)
        {
            var membership = await GetMembership(projectId);
            if (membership == null)
                return Forbid();

            var userId = GetUserId();

            var resource = new ProjectResource
            {
                ProjectId = projectId,
                Title = title,
                Type = type,
                UploadedById = userId
            };

            if (type == ResourceType.Document || type == ResourceType.PastPaper)
            {
                if (file == null || file.Length == 0)
                    return BadRequest();

                var uploadsFolder = Path.Combine("wwwroot/uploads/projects", projectId.ToString());
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var fullPath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream);

                resource.FilePath = $"/uploads/projects/{projectId}/{fileName}";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(url))
                    return BadRequest();

                resource.Url = url;
            }

            _context.ProjectResources.Add(resource);
            await _context.SaveChangesAsync();

            return RedirectToAction("Project", new { id = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TrackResourceView(int resourceId)
        {
            var userId = GetUserId();

            var resource = await _context.ProjectResources
                .Include(r => r.Project)
                .FirstOrDefaultAsync(r => r.Id == resourceId);

            if (resource == null)
                return NotFound();

            var isMember = await _context.ProjectMembers
                .AnyAsync(pm =>
                    pm.ProjectId == resource.ProjectId &&
                    pm.StudentId == userId);

            if (!isMember)
                return Forbid();

            var alreadyViewed = await _context.ResourceViews
                .AnyAsync(v =>
                    v.ResourceId == resourceId &&
                    v.StudentId == userId);

            if (!alreadyViewed)
            {
                _context.ResourceViews.Add(new ResourceView
                {
                    ResourceId = resourceId,
                    StudentId = userId
                });

                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDeadline(int projectId,
                                                     string title,
                                                     DateTime dueDate)
        {
            var userId = GetUserId();

            var isOwner = await _context.StudyProjects
                .AnyAsync(p =>
                    p.Id == projectId &&
                    p.OwnerId == userId);

            if (!isOwner)
                return Forbid();

            var deadline = new ProjectDeadline
            {
                ProjectId = projectId,
                Title = title,
                DueDate = dueDate
            };

            _context.ProjectDeadlines.Add(deadline);
            await _context.SaveChangesAsync();

            return RedirectToAction("Project", new { id = projectId });
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingInvitationCount()
        {
            var userId = GetUserId();

            var count = await _context.ProjectInvitations
                .CountAsync(i => i.StudentId == userId && i.Status == "Pending");

            return Json(new { count });
        }
    }
}