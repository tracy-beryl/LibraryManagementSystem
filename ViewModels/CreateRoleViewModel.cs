using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class CreateRoleViewModel
    {
        public string Id { get; set; }
        [Required]
        public string RoleName { get; set; }

        [Display(Name = "IsSelected?")]
        public bool IsSelected { get; set; }
    }
}
