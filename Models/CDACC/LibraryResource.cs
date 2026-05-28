using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models.CDACC
{
    public class LibraryResource
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ReferenceNumber { get; set; }

        public ResourceType Type { get; set; }

        public string UrlOrFilePath { get; set; }

        public string Author { get; set; }

        public string ISBN { get; set; }

        public string CoverImagePath { get; set; }

        public bool IsActive { get; set; } = true;

        public double DurationSeconds { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedByUserId { get; set; }

        public ICollection<ResourceCompetency> ResourceCompetencies { get; set; }
    }
}