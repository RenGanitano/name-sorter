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

    [Theory]
    [InlineData("Smith", "Smith")]
    [InlineData("smith", "SMITH")]
    public void Compare_LastNamesEqualIgnoringCase_OrdersByFirstGivenName(string leftLastName, string rightLastName)
    {
        var alice = new PersonName(["Alice"], leftLastName);
        var bob = new PersonName(["Bob"], rightLastName);

        comparer.Compare(alice, bob).Should().BeNegative();
    }

    [Fact]
    public void Compare_SameLastNameAndFirstGiven_OrdersBySecondGivenName()
    {
        var adamAaron = new PersonName(["Adam", "Aaron"], "Smith");
        var adamBruce = new PersonName(["Adam", "Bruce"], "Smith");

        comparer.Compare(adamAaron, adamBruce).Should().BeNegative();
    }

    [Theory]
    [InlineData(new[] { "Alice", "Marie" }, "Smith", new[] { "Alice", "Marie" }, "Smith")] // identical
    [InlineData(new[] { "ALICE" }, "SMITH", new[] { "alice" }, "smith")]                   // equal ignoring case
    public void Compare_EquivalentNames_ReturnsZero(
        string[] leftGiven, string leftLast, string[] rightGiven, string rightLast)
    {
        var left = new PersonName(leftGiven, leftLast);
        var right = new PersonName(rightGiven, rightLast);

        comparer.Compare(left, right).Should().Be(0);
    }

    [Fact]
    public void Compare_PunctuationAndAccents_UsesOrdinalCaseInsensitiveOrdering()
    {
        var apostrophe = new PersonName(["Alice"], "O'Neil");
        var accent = new PersonName(["Alice"], "Óneil");

        comparer.Compare(apostrophe, accent).Should().BeNegative();
    }

    [Fact]
    public void Compare_GivenNamePrefix_OrdersShorterNameFirst()
    {
        var shorter = new PersonName(["Adam"], "Smith");
        var longer = new PersonName(["Adam", "Bruce"], "Smith");

        comparer.Compare(shorter, longer).Should().BeNegative();
    }

    [Theory]
    [InlineData(true, false, -1)] // left null, right present -> nulls sort first
    [InlineData(false, true, 1)]  // left present, right null
    [InlineData(true, true, 0)]   // both null -> equal
    public void Compare_NullOperands_SortNullsFirst(bool leftNull, bool rightNull, int expectedSign)
    {
        var name = new PersonName(["John"], "Doe");

        var result = comparer.Compare(leftNull ? null : name, rightNull ? null : name);

        Math.Sign(result).Should().Be(expectedSign);
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
