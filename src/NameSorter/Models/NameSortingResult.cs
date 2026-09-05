namespace NameSorter.Models;

/// <summary>
/// Describes the outcome of processing an input file.
/// </summary>
public sealed record NameSortingResult(
    IList<PersonName> SortedNames,
    IReadOnlyList<InvalidName> InvalidNames);

/// <summary>
/// Describes an input line that could not be parsed as a name.
/// </summary>
public sealed record InvalidName(int LineNumber, string Value, string Reason);