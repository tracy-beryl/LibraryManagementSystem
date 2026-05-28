using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class ProjectInvitation
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public StudyProject Project { get; set; }

        public string StudentId { get; set; }
        public ApplicationUser Student { get; set; }

        public string Status { get; set; } // Pending, Accepted, Rejected

        public DateTime SentAt { get; set; } = DateTime.Now;
    }


}
