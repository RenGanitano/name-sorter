using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// Parses a raw name string into a <see cref="PersonName"/>.
/// A valid name has 2–4 parts (1–3 given names + 1 last name). The last name may span
/// several tokens when it starts with French surname particles (e.g. "de la Durantaye",
/// "Le Blanc"); those particles belong to the last name and do not count as given names.
/// </summary>
public class NameParser : INameParser
{
    private static readonly HashSet<string> SurnameParticles = new(StringComparer.OrdinalIgnoreCase)
    {
        "de", "du", "des", "d'",
        "le", "la", "les", "l'",
        "saint", "sainte", "st", "st.", "ste", "ste.",
    };

    public NameParseResult Parse(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
            return new NameParseResult(null, null);

        var tokens = rawName.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

        // The final token is always part of the last name; walk backwards absorbing any
        // particles that precede it so "Louis de la Durantaye" yields last name "de la Durantaye".
        var lastNameStart = tokens.Length - 1;
        while (lastNameStart > 0 && SurnameParticles.Contains(tokens[lastNameStart - 1]))
        {
            lastNameStart--;
        }

        var givenNames = tokens[..lastNameStart].ToList();
        var lastName = string.Join(' ', tokens[lastNameStart..]);

        // The whole last name counts as a single part regardless of how many tokens it spans.
        var partCount = givenNames.Count + 1;
        if (partCount < 2 || partCount > 4)
        {
            return new NameParseResult(null, $"expected 2-4 parts, got {partCount}");
        }

        return new NameParseResult(new PersonName(givenNames, lastName), null);
    }
}
