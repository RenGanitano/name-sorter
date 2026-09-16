using System.Globalization;
using NameSorter.Models;

namespace NameSorter.Comparers;

/// <summary>
/// Compares <see cref="PersonName"/> instances by last name first, then by given names left-to-right.
/// By default names are compared case-insensitively using French-Canadian (fr-CA) collation so
/// accented letters sort beside their unaccented forms (e.g. "Côté" next to "Cote"). A last name
/// that starts with a particle ("Le Blanc", "de la Durantaye") is sorted as written, under the particle.
/// </summary>
public class LastNameFirstComparer : IComparer<PersonName>
{
    private readonly StringComparer comparer;

    public LastNameFirstComparer()
        : this(StringComparer.Create(CultureInfo.GetCultureInfo("fr-CA"), ignoreCase: true))
    {
    }

    public LastNameFirstComparer(StringComparer comparer)
    {
        this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    public int Compare(PersonName? x, PersonName? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        // Compare last names first
        var lastNameComparison = this.comparer.Compare(x.LastName, y.LastName);
        if (lastNameComparison != 0)
            return lastNameComparison;

        // Tie-break by given names, left-to-right
        var minGivenNames = Math.Min(x.GivenNames.Count, y.GivenNames.Count);
        for (var i = 0; i < minGivenNames; i++)
        {
            var givenNameComparison = this.comparer.Compare(x.GivenNames[i], y.GivenNames[i]);
            if (givenNameComparison != 0)
                return givenNameComparison;
        }

        // If all matching given names are equal, shorter name comes first
        return x.GivenNames.Count.CompareTo(y.GivenNames.Count);
    }
}
