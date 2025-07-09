using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Staff>().HasData(

                new Staff
                {
                    Id = 1,
                    Email = "moyimeso@gmail.com",
                    StaffName = "Moyi Meso",
                    Department = "Software Systems",

                },

                   new Staff
                   {
                       Id = 2,
                       Email = "leahkateb@gmail.com",
                       StaffName = "Leah Kateb",
                       Department = "Information Systems",

                   }
                );

            modelBuilder.Entity<Admin>().HasData(

                new Admin
                {
                    Id = 1,
                    Email = "johndoe@gmail.com",
                    AdminName = "John Doe",

                },

                   new Admin
                   {
                       Id = 2,
                       Email = "emilyharris@gmail.com",
                       AdminName = "Emily Harris",

                   }
                );

            
        }
    }
}
