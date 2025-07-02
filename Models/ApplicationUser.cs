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
        public string StudentId { get; set; }

        public ICollection<Loan> Loans { get; set; }
    }
}
