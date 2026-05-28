using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class ResourceItemViewModel
    {
        public int ResourceId { get; set; }
        public string Title { get; set; }
        public ResourceType Type { get; set; }
        public bool IsCompleted { get; set; }
    }
}
