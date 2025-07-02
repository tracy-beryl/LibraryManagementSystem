using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class Staff
    {
        public int Id { get; set; }
        [Required]
        public string StaffName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string Password { get; set; }
    }
}
