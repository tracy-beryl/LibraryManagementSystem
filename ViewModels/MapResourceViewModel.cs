using LibraryManagementSystem.Models.CDACC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class MapResourceViewModel
    {
        public LibraryResource Resource { get; set; }
        public List<CompetencyStandard> Standards { get; set; }
        public List<int> SelectedStandards { get; set; }
    }
}
