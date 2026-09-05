using AwesomeAssertions;
using NameSorter.Models;

namespace NameSorter.Tests;

public class PersonNameTests
{
    [Fact]
    public void ToString_WithOneGivenName_ReturnsCorrectFormat()
    {
        var name = new PersonName(["Janet"], "Parsons");

        name.ToString().Should().Be("Janet Parsons");
    }

    [Fact]
    public void ToString_WithTwoGivenNames_ReturnsCorrectFormat()
    {
        var name = new PersonName(["Adonis", "Julius"], "Archer");

        name.ToString().Should().Be("Adonis Julius Archer");
    }

    [Fact]
    public void ToString_WithThreeGivenNames_ReturnsCorrectFormat()
    {
        var name = new PersonName(["Hunter", "Uriah", "Mathew"], "Clarke");

        name.ToString().Should().Be("Hunter Uriah Mathew Clarke");
    }

    [Fact]
    public void Constructor_NullGivenNames_ThrowsArgumentNullException()
    {
        Action act = () => new PersonName(null!, "Smith");

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_NullLastName_ThrowsArgumentNullException()
    {
        Action act = () => new PersonName(["John"], null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GivenNames_ExposesReadOnlyList()
    {
        var givenNames = new List<string> { "John" };
        var name = new PersonName(givenNames, "Smith");

        name.GivenNames.Should().BeAssignableTo<IReadOnlyList<string>>();
    }
}
