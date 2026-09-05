namespace NameSorter.Models;

/// <summary>
/// Represents a person's name with one or more given names and a last name.
/// </summary>
public class PersonName
{
    public IReadOnlyList<string> GivenNames { get; }
    public string LastName { get; }

    public PersonName(IReadOnlyList<string> givenNames, string lastName)
    {
        GivenNames = givenNames ?? throw new ArgumentNullException(nameof(givenNames));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
    }

    /// <summary>
    /// Returns the full name as "GivenName1 [GivenName2] [GivenName3] LastName".
    /// </summary>
    public override string ToString()
    {
        return string.Join(" ", GivenNames.Append(LastName));
    }
}
