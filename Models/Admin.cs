using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class Admin
    {
        public int Id { get; set; }
        [Required]
        public string AdminName { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }


    }
}
