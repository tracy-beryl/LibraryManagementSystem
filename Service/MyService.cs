using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Service
{
    public class MyService
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyService(LibraryDbContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> IsStudent(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var myRoleId = user.RoleId;

                if (!string.IsNullOrEmpty(myRoleId))
                {
                    var role = _context.Roles.FirstOrDefault(u => u.Id == myRoleId);

                    if (role != null)
                    {
                        return role.IsStudent;
                    }
                }
            }

            return false; 
        }

    }
}
