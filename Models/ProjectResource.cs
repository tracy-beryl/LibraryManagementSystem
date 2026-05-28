using LibraryManagementSystem.Models.CDACC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class ProjectResource
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public string Title { get; set; }

        public ResourceType Type { get; set; }

        // For uploaded documents & past papers
        public string FilePath { get; set; }

        // For external links & YouTube
        public string Url { get; set; }

        public string UploadedById { get; set; }
        public StudyProject Project { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ResourceView> Views { get; set; }
        public ICollection<ResourceCompetency> ResourceCompetencies { get; set; }
    }

}
