using AwesomeAssertions;
using NameSorter.Models;

namespace NameSorter.Tests;

public class PersonNameTests
{
    [Theory]
    [InlineData(new[] { "Janet" }, "Parsons", "Janet Parsons")]
    [InlineData(new[] { "Adonis", "Julius" }, "Archer", "Adonis Julius Archer")]
    [InlineData(new[] { "Hunter", "Uriah", "Mathew" }, "Clarke", "Hunter Uriah Mathew Clarke")]
    public void ToString_JoinsGivenNamesThenLastName(string[] givenNames, string lastName, string expected)
    {
        var name = new PersonName(givenNames, lastName);

        name.ToString().Should().Be(expected);
    }

    [Fact]
    public void Constructor_NullGivenNames_ThrowsArgumentNullException()
    {
        Action act = () => new PersonName(null!, "Smith");

        act.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("givenNames");
    }

    [Fact]
    public void Constructor_NullLastName_ThrowsArgumentNullException()
    {
        Action act = () => new PersonName(["John"], null!);

        act.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("lastName");
    }

    [Fact]
    public void GivenNames_ExposesReadOnlyList()
    {
        var givenNames = new List<string> { "John" };
        var name = new PersonName(givenNames, "Smith");

        name.GivenNames.Should().BeAssignableTo<IReadOnlyList<string>>();
    }
}
