using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    public class StudentDashboardController : Controller
    {
        private readonly LibraryDbContext _libraryDbContext;

        public StudentDashboardController(LibraryDbContext libraryDbContext)
        {
            _libraryDbContext = libraryDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        
    }
}
