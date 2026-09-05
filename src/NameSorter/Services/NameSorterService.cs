using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// Sorts names using an injected <see cref="IComparer{PersonName}"/> strategy.
/// </summary>
public class NameSorterService : INameSorter
{
    private readonly IComparer<PersonName> comparer;

    public NameSorterService(IComparer<PersonName> comparer)
    {
        this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    public IList<PersonName> Sort(IList<PersonName> names)
    {
        return names.OrderBy(name => name, this.comparer).ToList();
    }
}
