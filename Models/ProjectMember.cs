using System;

namespace LibraryManagementSystem.Models
{
    public class ProjectMember
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public StudyProject Project { get; set; }

        public string StudentId { get; set; }
        public ApplicationUser Student { get; set; }

        public string Role { get; set; } // Owner / Member

        // Tracks when this member last viewed the project chat
        public DateTime LastReadAt { get; set; } = DateTime.UtcNow;
    }
}