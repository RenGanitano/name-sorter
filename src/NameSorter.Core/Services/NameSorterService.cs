using NameSorter.Comparers;
using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// Sorts names using an injected <see cref="IComparer{PersonName}"/> strategy.
/// A descending sort wraps that comparer in a <see cref="DescendingComparer{T}"/>.
/// </summary>
public class NameSorterService : INameSorter
{
    private readonly IComparer<PersonName> comparer;

    public NameSorterService(IComparer<PersonName> comparer)
    {
        this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    public IList<PersonName> Sort(IList<PersonName> names, SortDirection direction = SortDirection.Ascending)
    {
        var effectiveComparer = direction == SortDirection.Descending
            ? new DescendingComparer<PersonName>(this.comparer)
            : this.comparer;

        return names.OrderBy(name => name, effectiveComparer).ToList();
    }
}
