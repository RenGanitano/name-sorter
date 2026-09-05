using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// Parses a raw name string into a <see cref="PersonName"/>.
/// </summary>
public interface INameParser
{
    /// <summary>
    /// Parses a raw name string. Returns null if the name is invalid.
    /// </summary>
    PersonName? Parse(string rawName);
}
