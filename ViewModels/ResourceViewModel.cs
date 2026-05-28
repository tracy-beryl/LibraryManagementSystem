using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class ResourceViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ResourceType Type { get; set; }
        public string FilePath { get; set; }
        public string Url { get; set; }
        public int ViewCount { get; set; }
    }
}
