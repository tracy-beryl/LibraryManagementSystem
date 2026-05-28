using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Service
{
    public class MyService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public MyService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> IsStudent(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            return await _userManager.IsInRoleAsync(user, "Student");
        }
    }
}