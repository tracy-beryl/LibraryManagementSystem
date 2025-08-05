using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        
        
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName { get; set; }
        public string IdentificationNumber { get; set; }

        public string RoleId { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Loan> Loans { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLoginDate { get; set; }
        
    }
}
