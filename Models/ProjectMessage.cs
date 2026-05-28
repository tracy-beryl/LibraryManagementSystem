using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class ProjectMessage
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public string SenderId { get; set; }

        public string Message { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; }

        // Navigation properties
        public StudyProject Project { get; set; }
        public ApplicationUser Sender { get; set; }
    }


}
