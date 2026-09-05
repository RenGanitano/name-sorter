using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// Parses a raw name string into a <see cref="PersonName"/>.
/// A valid name has 2–4 parts (1–3 given names + 1 last name).
/// </summary>
public class NameParser : INameParser
{
    public NameParseResult Parse(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
            return new NameParseResult(null, null);

        var parts = rawName.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2 || parts.Length > 4)
        {
            return new NameParseResult(null, $"expected 2-4 parts, got {parts.Length}");
        }

        var givenNames = parts[..^1].ToList();
        var lastName = parts[^1];

        return new NameParseResult(new PersonName(givenNames, lastName), null);
    }
}
