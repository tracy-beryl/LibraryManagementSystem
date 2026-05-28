using System.Collections.Generic;

namespace LibraryManagementSystem.ViewModels
{
    public class LecturerMyResourcesViewModel
    {
        public string SearchTerm { get; set; }
        public string UnitCode { get; set; }
        public string ResourceType { get; set; }
        public string Status { get; set; }
        public bool MyUploadsOnly { get; set; }

        public List<string> UnitCodes { get; set; } = new List<string>();
        public List<string> ResourceTypes { get; set; } = new List<string>();

        public List<LecturerResourceListItemViewModel> Resources { get; set; } = new List<LecturerResourceListItemViewModel>();
    }
}