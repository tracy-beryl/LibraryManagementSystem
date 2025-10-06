using Bumbe.Library.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bumbe.Library.Infrastructure;

public interface IReferenceNumberGenerator
{
    string Generate(string institutionCode, string branchCode, string collectionCode, int year, long sequence);
}

internal sealed class ReferenceNumberGenerator : IReferenceNumberGenerator
{
    public string Generate(string institutionCode, string branchCode, string collectionCode, int year, long sequence)
    {
        var seq = sequence.ToString().PadLeft(6, '0');
        var baseString = $"{institutionCode}-{branchCode}-{collectionCode}-{year}-{seq}";
        var digits = new string(baseString.Where(char.IsDigit).ToArray());
        var check = ComputeLuhn(digits);
        return $"{baseString}-{check}";
    }

    private static int ComputeLuhn(string number)
    {
        var sum = 0;
        var alt = false;
        for (var i = number.Length - 1; i >= 0; i--)
        {
            var n = number[i] - '0';
            if (alt)
            {
                n *= 2;
                if (n > 9) n -= 9;
            }
            sum += n;
            alt = !alt;
        }
        return (10 - (sum % 10)) % 10;
    }
}
public sealed class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    public DbSet<BibliographicRecord> BibliographicRecords => Set<BibliographicRecord>();
    public DbSet<ItemCopy> ItemCopies => Set<ItemCopy>();
    public DbSet<Patron> Patrons => Set<Patron>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<EResource> EResources => Set<EResource>();
    public DbSet<PastPaper> PastPapers => Set<PastPaper>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BibliographicRecord>(b =>
        {
            b.HasKey(x => x.Id);
            b.OwnsMany(x => x.Copies, c =>
            {
                c.WithOwner();
                c.HasKey(x => x.Id);
                c.Property(x => x.Barcode).HasMaxLength(64);
                c.Property(x => x.CallNumber).HasMaxLength(64);
            });
        });

        modelBuilder.Entity<Patron>(p =>
        {
            p.HasKey(x => x.Id);
            p.HasIndex(x => x.CardId).IsUnique();
        });

        modelBuilder.Entity<Loan>(l =>
        {
            l.HasKey(x => x.Id);
            l.HasIndex(x => new { x.ItemCopyId, x.ReturnedAtUtc });
        });
    }
}

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<LibraryDbContext>(options => options.UseInMemoryDatabase("BumbeLibrary"));
        services.AddSingleton<IReferenceNumberGenerator, ReferenceNumberGenerator>();
        return services;
    }
}
