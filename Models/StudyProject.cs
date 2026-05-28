using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class StudyProject
    {
        public int Id { get; set; }

        [Required]
        public string ProjectName { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<ProjectResource> ProjectResources { get; set; } = new List<ProjectResource>();
        public ICollection<ProjectDeadline> ProjectDeadlines { get; set; } = new List<ProjectDeadline>();
        public ICollection<ProjectMessage> ProjectMessages { get; set; } = new List<ProjectMessage>();
    }

}
