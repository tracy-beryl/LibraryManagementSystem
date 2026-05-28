
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using System.Linq;

namespace LibraryManagementSystem.Filters
{
    public class StudentOnlyAttribute : TypeFilterAttribute
    {
        public StudentOnlyAttribute() : base(typeof(StudentOnlyFilter))
        {
        }
    }

    public class StudentOnlyFilter : IAsyncActionFilter
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LibraryDbContext _context;

        public StudentOnlyFilter(
            UserManager<ApplicationUser> userManager,
            LibraryDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            var user = await _userManager.GetUserAsync(context.HttpContext.User);

            if (user == null || !user.IsActive)
            {
                context.Result = new ForbidResult();
                return;
            }

            var hasStudentProfile = _context.StudentProfiles
                .Any(sp => sp.UserId == user.Id);

            if (!hasStudentProfile)
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}