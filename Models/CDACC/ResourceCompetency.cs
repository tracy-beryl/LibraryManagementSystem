using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models.CDACC
{
    public class ResourceCompetency
    {
        public int ResourceId { get; set; }
        public LibraryResource Resource { get; set; }

        public int CompetencyStandardId { get; set; }
        public CompetencyStandard CompetencyStandard { get; set; }

        // Audit fields
        public string MappedByUserId { get; set; }
        public ApplicationUser MappedByUser { get; set; }

        public DateTime MappedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
