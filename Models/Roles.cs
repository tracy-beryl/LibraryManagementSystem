using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class Roles 
    {
        public string Id { get; set; }
        public bool IsStudent { get; set; }
        public string RoleName { get; set; }
    }
}
