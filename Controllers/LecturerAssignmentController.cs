using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class LecturerAssignmentController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LecturerAssignmentController(
            LibraryDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var assignments = await _context.LecturerUnitAssignments
                .Include(a => a.LecturerUser)
                .OrderBy(a => a.Program)
                .ThenBy(a => a.Level)
                .ThenBy(a => a.Semester)
                .ThenBy(a => a.UnitCode)
                .ToListAsync();

            return View(assignments);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var lecturers = await _userManager.GetUsersInRoleAsync("Lecturer");

            ViewBag.Lecturers = lecturers;

            ViewBag.Units = await _context.CompetencyStandards
                .Where(c => c.IsActive)
                .Select(c => new LecturerUnitOptionViewModel
                {
                    Program = c.Program,
                    Level = c.Level,
                    Semester = c.Semester,
                    UnitCode = c.UnitCode,
                    UnitName = c.UnitName
                })
                .Distinct()
                .OrderBy(c => c.Program)
                .ThenBy(c => c.Level)
                .ThenBy(c => c.Semester)
                .ThenBy(c => c.UnitCode)
                .ToListAsync();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLecturerAssignmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Lecturers = await _userManager.GetUsersInRoleAsync("Lecturer");
                ViewBag.Units = await _context.CompetencyStandards
                    .Where(c => c.IsActive)
                    .Select(c => new LecturerUnitOptionViewModel
                    {
                        Program = c.Program,
                        Level = c.Level,
                        Semester = c.Semester,
                        UnitCode = c.UnitCode,
                        UnitName = c.UnitName
                    })
                    .Distinct()
                    .OrderBy(c => c.Program)
                    .ThenBy(c => c.Level)
                    .ThenBy(c => c.Semester)
                    .ThenBy(c => c.UnitCode)
                    .ToListAsync();

                return View(model);
            }

            var validUnit = await _context.CompetencyStandards.AnyAsync(c =>
                c.IsActive &&
                c.Program == model.Program &&
                c.Level == model.Level &&
                c.Semester == model.Semester &&
                c.UnitCode == model.UnitCode &&
                c.UnitName == model.UnitName);

            if (!validUnit)
            {
                ModelState.AddModelError("", "Selected unit is not valid.");

                ViewBag.Lecturers = await _userManager.GetUsersInRoleAsync("Lecturer");
                ViewBag.Units = await _context.CompetencyStandards
                    .Where(c => c.IsActive)
                    .Select(c => new LecturerUnitOptionViewModel
                    {
                        Program = c.Program,
                        Level = c.Level,
                        Semester = c.Semester,
                        UnitCode = c.UnitCode,
                        UnitName = c.UnitName
                    })
                    .Distinct()
                    .OrderBy(c => c.Program)
                    .ThenBy(c => c.Level)
                    .ThenBy(c => c.Semester)
                    .ThenBy(c => c.UnitCode)
                    .ToListAsync();

                return View(model);
            }

            bool exists = await _context.LecturerUnitAssignments.AnyAsync(a =>
                a.LecturerUserId == model.LecturerUserId &&
                a.Program == model.Program &&
                a.Level == model.Level &&
                a.Semester == model.Semester &&
                a.UnitCode == model.UnitCode &&
                a.IsActive);

            if (exists)
            {
                ModelState.AddModelError("", "This lecturer is already assigned to that unit.");

                ViewBag.Lecturers = await _userManager.GetUsersInRoleAsync("Lecturer");
                ViewBag.Units = await _context.CompetencyStandards
                    .Where(c => c.IsActive)
                    .Select(c => new LecturerUnitOptionViewModel
                    {
                        Program = c.Program,
                        Level = c.Level,
                        Semester = c.Semester,
                        UnitCode = c.UnitCode,
                        UnitName = c.UnitName
                    })
                    .Distinct()
                    .OrderBy(c => c.Program)
                    .ThenBy(c => c.Level)
                    .ThenBy(c => c.Semester)
                    .ThenBy(c => c.UnitCode)
                    .ToListAsync();

                return View(model);
            }

            var assignment = new LecturerUnitAssignment
            {
                LecturerUserId = model.LecturerUserId,
                Program = model.Program,
                Level = model.Level,
                Semester = model.Semester,
                UnitCode = model.UnitCode,
                UnitName = model.UnitName,
                IsActive = true
            };

            _context.LecturerUnitAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Lecturer assigned successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var assignment = await _context.LecturerUnitAssignments.FindAsync(id);
            if (assignment == null)
                return NotFound();

            assignment.IsActive = false;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Assignment deactivated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}