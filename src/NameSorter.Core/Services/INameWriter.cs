using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// Writes a list of sorted names to a destination.
/// </summary>
public interface INameWriter
{
    void Write(IList<PersonName> names);
}
