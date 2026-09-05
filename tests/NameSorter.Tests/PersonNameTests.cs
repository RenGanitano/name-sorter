using NameSorter.Models;

namespace NameSorter.Tests;

public class PersonNameTests
{
    [Fact]
    public void ToString_WithOneGivenName_ReturnsCorrectFormat()
    {
        var name = new PersonName(new List<string> { "Janet" }, "Parsons");

        Assert.Equal("Janet Parsons", name.ToString());
    }

    [Fact]
    public void ToString_WithTwoGivenNames_ReturnsCorrectFormat()
    {
        var name = new PersonName(new List<string> { "Adonis", "Julius" }, "Archer");

        Assert.Equal("Adonis Julius Archer", name.ToString());
    }

    [Fact]
    public void ToString_WithThreeGivenNames_ReturnsCorrectFormat()
    {
        var name = new PersonName(new List<string> { "Hunter", "Uriah", "Mathew" }, "Clarke");

        Assert.Equal("Hunter Uriah Mathew Clarke", name.ToString());
    }

    [Fact]
    public void Constructor_ThrowsOnNullGivenNames()
    {
        Assert.Throws<ArgumentNullException>(() => new PersonName(null!, "Smith"));
    }

    [Fact]
    public void Constructor_ThrowsOnNullLastName()
    {
        Assert.Throws<ArgumentNullException>(() => new PersonName(new List<string> { "John" }, null!));
    }

    [Fact]
    public void GivenNames_IsReadOnly()
    {
        var givenNames = new List<string> { "John" };
        var name = new PersonName(givenNames, "Smith");

        Assert.IsAssignableFrom<IReadOnlyList<string>>(name.GivenNames);
    }
}
