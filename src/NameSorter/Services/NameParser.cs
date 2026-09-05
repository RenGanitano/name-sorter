using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// Parses a raw name string into a <see cref="PersonName"/>.
/// A valid name has 2–4 parts (1–3 given names + 1 last name).
/// </summary>
public class NameParser : INameParser
{
    public PersonName? Parse(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
            return null;

        var parts = rawName.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2 || parts.Length > 4)
        {
            Console.Error.WriteLine($"Warning: Invalid name \"{rawName}\" — expected 2–4 parts, got {parts.Length}. Skipping.");
            return null;
        }

        var givenNames = parts[..^1].ToList();
        var lastName = parts[^1];

        return new PersonName(givenNames, lastName);
    }
}
