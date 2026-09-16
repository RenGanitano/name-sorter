using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// Parses and sorts raw name strings, reporting any that could not be parsed.
/// </summary>
public interface INameSortingProcessor
{
    /// <summary>
    /// Parses each entry of <paramref name="rawNames"/>, sorts the valid names in the given
    /// direction, and reports invalid entries by their 1-based position in the input.
    /// Blank entries are ignored but still count toward the position.
    /// </summary>
    NameSortingResult Process(IList<string> rawNames, SortDirection direction = SortDirection.Ascending);
}
