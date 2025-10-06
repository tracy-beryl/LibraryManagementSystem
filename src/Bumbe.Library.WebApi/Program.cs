using Bumbe.Library.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok", timeUtc = DateTime.UtcNow }))
    .WithName("Health")
    .WithOpenApi();

app.MapGet("/books", async (LibraryDbContext db) =>
{
    var results = await db.BibliographicRecords
        .AsNoTracking()
        .Select(b => new { b.Id, b.Title, b.Author, b.PublicationYear })
        .ToListAsync();
    return Results.Ok(results);
}).WithName("ListBooks").WithOpenApi();

app.MapGet("/softcopies", async (LibraryDbContext db) =>
{
    var results = await db.EResources
        .AsNoTracking()
        .Select(e => new { e.Id, e.Title, e.AccessLevel })
        .ToListAsync();
    return Results.Ok(results);
}).WithName("ListEResources").WithOpenApi();

app.MapGet("/past-papers", async (LibraryDbContext db) =>
{
    var results = await db.PastPapers
        .AsNoTracking()
        .Select(p => new { p.Id, p.CourseCode, p.Year, p.AccessLevel })
        .ToListAsync();
    return Results.Ok(results);
}).WithName("ListPastPapers").WithOpenApi();

app.MapPost("/books", async (LibraryDbContext db, IReferenceNumberGenerator refGen, IConfiguration config, BookCreateRequest request) =>
{
    var record = new Bumbe.Library.Domain.BibliographicRecord
    {
        Title = request.Title,
        Author = request.Author,
        Subject = request.Subject,
        PublicationYear = request.PublicationYear,
        Classification = request.Classification,
        ReferenceNumber = refGen.Generate(
            config["Institution:Code"] ?? "BNP",
            config["Institution:DefaultBranchCode"] ?? "MAIN",
            config["Institution:DefaultCollectionCode"] ?? "GEN",
            DateTime.UtcNow.Year,
            Random.Shared.Next(1, 999999))
    };
    db.BibliographicRecords.Add(record);
    await db.SaveChangesAsync();
    return Results.Created($"/books/{record.Id}", new { record.Id });
}).WithName("CreateBook").WithOpenApi();

app.Run();

public sealed record BookCreateRequest(
    string Title,
    string Author,
    string Subject,
    int PublicationYear,
    string Classification
);
