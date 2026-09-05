using NameSorter.Comparers;
using NameSorter.Models;

namespace NameSorter.Tests;

public class LastNameFirstComparerTests
{
    private readonly LastNameFirstComparer comparer = new();

    [Fact]
    public void Compare_DifferentLastNames_SortsByLastName()
    {
        var archer = new PersonName(new List<string> { "Adonis", "Julius" }, "Archer");
        var bentley = new PersonName(new List<string> { "Beau", "Tristan" }, "Bentley");

        Assert.True(comparer.Compare(archer, bentley) < 0);
        Assert.True(comparer.Compare(bentley, archer) > 0);
    }

    [Fact]
    public void Compare_SameLastName_SortsByFirstGivenName()
    {
        var alice = new PersonName(new List<string> { "Alice" }, "Smith");
        var bob = new PersonName(new List<string> { "Bob" }, "Smith");

        Assert.True(comparer.Compare(alice, bob) < 0);
    }

    [Fact]
    public void Compare_SameLastNameAndFirstGiven_SortsBySecondGivenName()
    {
        var adamAaron = new PersonName(new List<string> { "Adam", "Aaron" }, "Smith");
        var adamBruce = new PersonName(new List<string> { "Adam", "Bruce" }, "Smith");

        Assert.True(comparer.Compare(adamAaron, adamBruce) < 0);
    }

    [Fact]
    public void Compare_CaseInsensitive()
    {
        var upper = new PersonName(new List<string> { "ALICE" }, "SMITH");
        var lower = new PersonName(new List<string> { "alice" }, "smith");

        Assert.Equal(0, comparer.Compare(upper, lower));
    }

    [Fact]
    public void Compare_FewerGivenNames_ComesFirst_WhenPrefixMatches()
    {
        var shorter = new PersonName(new List<string> { "Adam" }, "Smith");
        var longer = new PersonName(new List<string> { "Adam", "Bruce" }, "Smith");

        Assert.True(comparer.Compare(shorter, longer) < 0);
    }

    [Fact]
    public void Compare_SameNameReturnsZero()
    {
        var name = new PersonName(new List<string> { "John" }, "Doe");

        Assert.Equal(0, comparer.Compare(name, name));
    }

    [Fact]
    public void Compare_NullHandling()
    {
        var name = new PersonName(new List<string> { "John" }, "Doe");

        Assert.True(comparer.Compare(null, name) < 0);
        Assert.True(comparer.Compare(name, null) > 0);
        Assert.Equal(0, comparer.Compare(null, null));
    }

    [Fact]
    public void Compare_FullSpecExample_SortsCorrectly()
    {
        var names = new List<PersonName>
        {
            new(new List<string> { "Janet" }, "Parsons"),
            new(new List<string> { "Vaughn" }, "Lewis"),
            new(new List<string> { "Adonis", "Julius" }, "Archer"),
            new(new List<string> { "Shelby", "Nathan" }, "Yoder"),
            new(new List<string> { "Marin" }, "Alvarez"),
            new(new List<string> { "London" }, "Lindsey"),
            new(new List<string> { "Beau", "Tristan" }, "Bentley"),
            new(new List<string> { "Leo" }, "Gardner"),
            new(new List<string> { "Hunter", "Uriah", "Mathew" }, "Clarke"),
            new(new List<string> { "Mikayla" }, "Lopez"),
            new(new List<string> { "Frankie", "Conner" }, "Ritter"),
        };

        names.Sort(comparer);

        var result = names.Select(n => n.ToString()).ToList();

        Assert.Equal(new List<string>
        {
            "Marin Alvarez",
            "Adonis Julius Archer",
            "Beau Tristan Bentley",
            "Hunter Uriah Mathew Clarke",
            "Leo Gardner",
            "Vaughn Lewis",
            "London Lindsey",
            "Mikayla Lopez",
            "Janet Parsons",
            "Frankie Conner Ritter",
            "Shelby Nathan Yoder",
        }, result);
    }
}
