namespace Bumbe.Library.Domain;

public sealed class Branch
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Code { get; init; } = string.Empty; // e.g., MAIN, TTI
    public string Name { get; init; } = string.Empty;
}

public sealed class BibliographicRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string ReferenceNumber { get; set; } = string.Empty; // e.g., BNP-MAIN-GEN-2025-001237-7
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string Classification { get; set; } = string.Empty; // DDC/CALL
    public List<ItemCopy> Copies { get; init; } = new();
}

public sealed class ItemCopy
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Barcode { get; set; } = string.Empty;
    public string CallNumber { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public ItemCopyStatus Status { get; set; } = ItemCopyStatus.Available;
}

public enum ItemCopyStatus
{
    Available = 0,
    OnLoan = 1,
    Lost = 2,
    Damaged = 3,
}

public sealed class Patron
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string CardId { get; set; } = string.Empty; // barcode/QR
    public string FullName { get; set; } = string.Empty;
    public string PatronType { get; set; } = string.Empty; // Student/Lecturer/Staff
    public string Department { get; set; } = string.Empty;
}

public sealed class Loan
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ItemCopyId { get; set; }
    public Guid PatronId { get; set; }
    public DateTime LoanedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime DueAtUtc { get; set; }
    public DateTime? ReturnedAtUtc { get; set; }
}

public sealed class EResource
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty; // storage key
    public string AccessLevel { get; set; } = "Public"; // Public/Campus/PatronOnly
}

public sealed class PastPaper
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string CourseCode { get; set; } = string.Empty;
    public int Year { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string AccessLevel { get; set; } = "Public";
}
