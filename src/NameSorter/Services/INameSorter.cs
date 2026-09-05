using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// Sorts a list of <see cref="PersonName"/> instances.
/// </summary>
public interface INameSorter
{
    IList<PersonName> Sort(IList<PersonName> names);
}
