

using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class BookViewModel

    { 
        public List<Book> Books { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public string StudentId { get; set; }
        public int BookId { get; set; }
        public int AvailableCopies { get; set; }
      
    }
}
