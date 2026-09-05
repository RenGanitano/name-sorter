using NameSorter.Models;

namespace NameSorter.Comparers;

/// <summary>
/// Compares <see cref="PersonName"/> instances by last name first, then by given names left-to-right, case-insensitive.
/// </summary>
public class LastNameFirstComparer : IComparer<PersonName>
{
    public int Compare(PersonName? x, PersonName? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        // Compare last names first (case-insensitive)
        var lastNameComparison = string.Compare(x.LastName, y.LastName, StringComparison.OrdinalIgnoreCase);
        if (lastNameComparison != 0)
            return lastNameComparison;

        // Tie-break by given names, left-to-right
        var minGivenNames = Math.Min(x.GivenNames.Count, y.GivenNames.Count);
        for (var i = 0; i < minGivenNames; i++)
        {
            var givenNameComparison = string.Compare(x.GivenNames[i], y.GivenNames[i], StringComparison.OrdinalIgnoreCase);
            if (givenNameComparison != 0)
                return givenNameComparison;
        }

        // If all matching given names are equal, shorter name comes first
        return x.GivenNames.Count.CompareTo(y.GivenNames.Count);
    }
}
