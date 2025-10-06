using Bumbe.Library.Domain;
using MediatR;

namespace Bumbe.Library.Application;

public static class ApplicationAssemblyMarker {}

// Query: list simple books
public sealed record ListBooksQuery() : IRequest<IReadOnlyList<BibliographicRecord>>;
