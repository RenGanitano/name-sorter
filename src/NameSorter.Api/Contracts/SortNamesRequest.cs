using NameSorter.Models;

namespace NameSorter.Api.Contracts;

/// <summary>
/// Request body for <c>POST /names/sort</c>: one raw name per entry, in the same
/// "given names then last name" format the CLI reads from a file.
/// </summary>
/// <param name="Names">Raw names to sort. Blank entries are ignored.</param>
/// <param name="Direction">Sort direction; defaults to ascending when omitted.</param>
public sealed record SortNamesRequest(
    IReadOnlyList<string>? Names,
    SortDirection Direction = SortDirection.Ascending);
