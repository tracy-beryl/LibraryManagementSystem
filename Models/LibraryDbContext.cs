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
        public DbSet<Roles> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Seed(); 

            
            modelBuilder.Entity<Roles>().HasData(
            new Roles { Id = "4b729351-7f04-4e51-9c7e-017fdf369e01", RoleName = "Admin", IsStudent = false },
            new Roles { Id = "5eba0279-cee8-40b6-92e9-fd54ba4a5b60", RoleName = "Staff", IsStudent = false },
            new Roles { Id = "25272b90-aa34-44d3-89cf-1ca24037ca92", RoleName = "Student", IsStudent = true }
        );

            var hasher = new PasswordHasher<ApplicationUser>();

            var adminUser = new ApplicationUser
            {
                Id = "8b8b820f-bf84-4ffe-87bd-be619179d833",
                UserName = "johndoe@gmail.com",
                NormalizedUserName = "JOHNDOE@GMAIL.COM",
                Email = "johndoe@gmail.com",
                NormalizedEmail = "JOHNDOE@GMAIL.COM",
                EmailConfirmed = true,
                FullName = "John Doe",
                RoleId = "e577164a-6f5f-4f2d-8f2d-f58bd6a1e8f8"
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

           
            

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);





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



