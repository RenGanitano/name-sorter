using NameSorter.Models;

namespace NameSorter.Tests;

public class PersonNameTests
{
    [Fact]
    public void ToString_WithOneGivenName_ReturnsCorrectFormat()
    {
        var name = new PersonName(["Janet"], "Parsons");

        Assert.Equal("Janet Parsons", name.ToString());
    }

    [Fact]
    public void ToString_WithTwoGivenNames_ReturnsCorrectFormat()
    {
        var name = new PersonName(["Adonis", "Julius"], "Archer");

        Assert.Equal("Adonis Julius Archer", name.ToString());
    }

    [Fact]
    public void ToString_WithThreeGivenNames_ReturnsCorrectFormat()
    {
        var name = new PersonName(["Hunter", "Uriah", "Mathew"], "Clarke");

        Assert.Equal("Hunter Uriah Mathew Clarke", name.ToString());
    }

    [Fact]
    public void Constructor_NullGivenNames_ThrowsArgumentNullException()
    {
        Action act = () => new PersonName(null!, "Smith");

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_NullLastName_ThrowsArgumentNullException()
    {
        Action act = () => new PersonName(["John"], null!);

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void GivenNames_ExposesReadOnlyList()
    {
        var givenNames = new List<string> { "John" };
        var name = new PersonName(givenNames, "Smith");

        Assert.IsAssignableFrom<IReadOnlyList<string>>(name.GivenNames);
    }
}
