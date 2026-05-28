using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class ResourceView
    {
        public int Id { get; set; }

        public int ResourceId { get; set; }

        public string StudentId { get; set; }

        public DateTime ViewedAt { get; set; } = DateTime.Now;
    }

}
