using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// Parses a raw name string into a <see cref="PersonName"/>.
/// </summary>
public interface INameParser
{
    /// <summary>
    /// Parses a raw name string and returns validation details when it is invalid.
    /// </summary>
    NameParseResult Parse(string rawName);
}
