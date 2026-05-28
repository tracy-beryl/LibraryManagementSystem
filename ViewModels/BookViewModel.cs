

using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class BookViewModel

    {
        public List<Book> AllBooks { get; set; }  // for table
        public List<Book> Books { get; set; }     // for modal dropdown

        public List<ApplicationUser> Users { get; set; }
        public string StudentId { get; set; }
        public int BookId { get; set; }
        public int AvailableCopies { get; set; }
        public string SelectedDepartment { get; set; }
        public string SelectedCategory { get; set; }
      

        public List<string> Departments { get; set; }
        public List<string> Categories { get; set; }

    }
}
