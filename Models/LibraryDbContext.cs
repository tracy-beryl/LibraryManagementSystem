using LibraryManagementSystem.Models.CDACC;
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
     IdentityDbContext<ApplicationUser, IdentityRole, string>
    {

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
                    : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<PastPapers> PastPapers { get; set; }

        public DbSet<SoftCopy> SoftCopies { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
        public DbSet<BookReservation> BookReservations { get; set; }
        public DbSet<BookSuggestion> BookSuggestions { get; set; }
        public DbSet<ProjectMember>  ProjectMembers { get; set; }
        public DbSet<StudyProject> StudyProjects { get; set; }

        public DbSet<ProjectDeadline> ProjectDeadlines { get; set; }
        public DbSet<ProjectResource> ProjectResources { get; set; }
        public DbSet<ProjectInvitation> ProjectInvitations { get; set; }

        public DbSet<ProjectMessage> ProjectMessages { get; set; }
        public DbSet<ResourceView> ResourceViews { get; set; }
        public DbSet<CompetencyStandard> CompetencyStandards { get; set; }
        public DbSet<LibraryResource> LibraryResources { get; set; }
        public DbSet<ResourceCompetency> ResourceCompetencies { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<StaffProfile> StaffProfiles { get; set; }
        public DbSet<PastPaperAttempt> PastPaperAttempts { get; set; }

        public DbSet<StudentResourceProgress> StudentResourceProgresses { get; set; }
        public DbSet<InventoryRecord> InventoryRecords { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<DamageReport> DamageReports { get; set; }

        public DbSet<LecturerUnitAssignment> LecturerUnitAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.Property(l => l.FrozenFineAmount)
                      .HasColumnType("decimal(18,2)");

                entity.Property(l => l.ReplacementCost)
                      .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<PaymentReceipt>(entity =>
            {
                entity.Property(p => p.Amount)
                      .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Loan>()
        .Property(l => l.FineAmountPaid)
        .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<BookReservation>()
                .HasOne(r => r.Book)
                .WithMany()
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookReservation>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BookSuggestion>()
                .HasOne(s => s.SuggestedBy)
                .WithMany()
                .HasForeignKey(s => s.SuggestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudyProject>()
            .HasOne(p => p.Owner)
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project)
                .WithMany(p => p.ProjectMembers)
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Student)
                .WithMany()
                .HasForeignKey(pm => pm.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectResource>()
           .Property(r => r.Type)
           .HasConversion<int>();
            modelBuilder.Entity<ResourceCompetency>()
           .HasKey(rc => new { rc.ResourceId, rc.CompetencyStandardId });

            modelBuilder.Entity<StudentProfile>()
        .HasOne(s => s.User)
        .WithOne(u => u.StudentProfile)
        .HasForeignKey<StudentProfile>(s => s.UserId);

            modelBuilder.Entity<StaffProfile>()
                .HasOne(s => s.User)
                .WithOne(u => u.StaffProfile)
                .HasForeignKey<StaffProfile>(s => s.UserId);

            modelBuilder.Entity<ProjectDeadline>()
            .HasOne(d => d.Project)
            .WithMany(p => p.ProjectDeadlines)
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Book>()
    .HasIndex(b => new { b.ISBN, b.Edition, b.Department })
    .IsUnique();


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



