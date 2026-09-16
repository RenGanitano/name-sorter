using NameSorter.Models;

namespace NameSorter.Api.Contracts;

/// <summary>
/// Response body for <c>POST /names/sort</c>.
/// </summary>
/// <param name="SortedNames">Valid names in sorted order, formatted as "GivenNames LastName".</param>
/// <param name="InvalidNames">
/// Entries that could not be parsed, with <see cref="InvalidName.LineNumber"/> being the
/// 1-based position in the request's <c>names</c> array.
/// </param>
public sealed record SortNamesResponse(
    IReadOnlyList<string> SortedNames,
    IReadOnlyList<InvalidName> InvalidNames);
