using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class LibraryDbContext : 
     IdentityDbContext<ApplicationUser>
    {

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
                    : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<PastPapers> PastPapers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Seed(); 

            // Seed Identity Roles
            var adminRole = new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN"
            };

            var staffRole = new IdentityRole
            {
                Id = "2",
                Name = "Staff",
                NormalizedName = "STAFF"
            };

            var studentRole = new IdentityRole
            {
                Id = "3",
                Name = "Student",
                NormalizedName = "STUDENT"
            };

            modelBuilder.Entity<IdentityRole>().HasData(adminRole, staffRole, studentRole);

            // Seed Admin & Staff users with password hashes
            var hasher = new PasswordHasher<ApplicationUser>();

            var adminUser = new ApplicationUser
            {
                Id = "A1",
                UserName = "johndoe@gmail.com",
                NormalizedUserName = "JOHNDOE@GMAIL.COM",
                Email = "johndoe@gmail.com",
                NormalizedEmail = "JOHNDOE@GMAIL.COM",
                EmailConfirmed = true,
                FullName = "John Doe"
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

            var staffUser = new ApplicationUser
            {
                Id = "S1",
                UserName = "moyimeso@gmail.com",
                NormalizedUserName = "MOYIMESO@GMAIL.COM",
                Email = "moyimeso@gmail.com",
                NormalizedEmail = "MOYIMESO@GMAIL.COM",
                EmailConfirmed = true,
                FullName = "Moyi Meso"
            };
            staffUser.PasswordHash = hasher.HashPassword(staffUser, "Moyi@123");

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser, staffUser);

            // Assign roles to users
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = "A1",
                    RoleId = "1" 
        },
                new IdentityUserRole<string>
                {
                    UserId = "S1",
                    RoleId = "2" 
        }
            );

            // Relationships for Loan remain unchanged
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

      

           
               
        }

        

    }



