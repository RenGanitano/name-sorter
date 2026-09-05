using AwesomeAssertions;
using NameSorter.Comparers;
using NameSorter.Models;

namespace NameSorter.Tests;

public class LastNameFirstComparerTests
{
    private readonly LastNameFirstComparer comparer = new();

    [Fact]
    public void Compare_DifferentLastNames_OrdersByLastName()
    {
        var archer = new PersonName(["Adonis", "Julius"], "Archer");
        var bentley = new PersonName(["Beau", "Tristan"], "Bentley");

        comparer.Compare(archer, bentley).Should().BeNegative();
        comparer.Compare(bentley, archer).Should().BePositive();
    }

    [Fact]
    public void Compare_SameLastName_OrdersByFirstGivenName()
    {
        var alice = new PersonName(["Alice"], "Smith");
        var bob = new PersonName(["Bob"], "Smith");

        comparer.Compare(alice, bob).Should().BeNegative();
    }

    [Fact]
    public void Compare_SameLastNameAndFirstGiven_OrdersBySecondGivenName()
    {
        var adamAaron = new PersonName(["Adam", "Aaron"], "Smith");
        var adamBruce = new PersonName(["Adam", "Bruce"], "Smith");

        comparer.Compare(adamAaron, adamBruce).Should().BeNegative();
    }

    [Fact]
    public void Compare_EqualNamesIgnoringCase_ReturnsZero()
    {
        var upper = new PersonName(["ALICE"], "SMITH");
        var lower = new PersonName(["alice"], "smith");

        comparer.Compare(upper, lower).Should().Be(0);
    }

    [Fact]
    public void Compare_LastNamesIgnoringCase_OrdersByGivenNames()
    {
        var alice = new PersonName(["Alice"], "smith");
        var bob = new PersonName(["Bob"], "SMITH");

        comparer.Compare(alice, bob).Should().BeNegative();
    }

    [Fact]
    public void Compare_PunctuationAndAccents_UsesOrdinalCaseInsensitiveOrdering()
    {
        var apostrophe = new PersonName(["Alice"], "O'Neil");
        var accent = new PersonName(["Alice"], "Óneil");

        comparer.Compare(apostrophe, accent).Should().BeNegative();
    }

    [Fact]
    public void Compare_DuplicateNames_ReturnsZero()
    {
        var first = new PersonName(["Alice", "Marie"], "Smith");
        var second = new PersonName(["Alice", "Marie"], "Smith");

        comparer.Compare(first, second).Should().Be(0);
    }

    [Fact]
    public void Compare_GivenNamePrefix_OrdersShorterNameFirst()
    {
        var shorter = new PersonName(["Adam"], "Smith");
        var longer = new PersonName(["Adam", "Bruce"], "Smith");

        comparer.Compare(shorter, longer).Should().BeNegative();
    }

    [Fact]
    public void Compare_SameName_ReturnsZero()
    {
        var name = new PersonName(["John"], "Doe");

        comparer.Compare(name, name).Should().Be(0);
    }

    [Fact]
    public void Compare_NullLeft_ReturnsNegative()
    {
        var name = new PersonName(["John"], "Doe");

        comparer.Compare(null, name).Should().BeNegative();
    }

    [Fact]
    public void Compare_NullRight_ReturnsPositive()
    {
        var name = new PersonName(["John"], "Doe");

        comparer.Compare(name, null).Should().BePositive();
    }

    [Fact]
    public void Compare_BothNull_ReturnsZero()
    {
        comparer.Compare(null, null).Should().Be(0);
    }

    [Fact]
    public void Compare_FullSpecificationExample_ReturnsExpectedOrder()
    {
        var names = new List<PersonName>
        {
            new(["Janet"], "Parsons"),
            new(["Vaughn"], "Lewis"),
            new(["Adonis", "Julius"], "Archer"),
            new(["Shelby", "Nathan"], "Yoder"),
            new(["Marin"], "Alvarez"),
            new(["London"], "Lindsey"),
            new(["Beau", "Tristan"], "Bentley"),
            new(["Leo"], "Gardner"),
            new(["Hunter", "Uriah", "Mathew"], "Clarke"),
            new(["Mikayla"], "Lopez"),
            new(["Frankie", "Conner"], "Ritter"),
        };

        names.Sort(comparer);

        var result = names.Select(n => n.ToString()).ToList();

        result.Should().Equal(
        new[]
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
        });
    }
}
