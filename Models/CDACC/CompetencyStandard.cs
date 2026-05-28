using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models.CDACC
{
    public class CompetencyStandard
    {
        public int Id { get; set; }

        public string Program { get; set; }
        public string Level { get; set; }

        public string UnitCode { get; set; }
        public string UnitName { get; set; }

        // Versioning
        public int VersionNumber { get; set; } = 1;

        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
        public DateTime? EffectiveTo { get; set; }

        public bool IsActive { get; set; } = true;
        public int Semester { get; set; }
        public ICollection<ResourceCompetency> ResourceCompetencies { get; set; }
    }
}
