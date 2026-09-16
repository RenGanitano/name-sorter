using AwesomeAssertions;
using NameSorter.Services;

namespace NameSorter.Tests;

public class NameParserTests
{
    private readonly NameParser parser = new();

    [Theory]
    [InlineData("Janet Parsons", "Parsons", new[] { "Janet" })]
    [InlineData("Adonis Julius Archer", "Archer", new[] { "Adonis", "Julius" })]
    [InlineData("Hunter Uriah Mathew Clarke", "Clarke", new[] { "Hunter", "Uriah", "Mathew" })]
    public void Parse_ValidName_SplitsGivenNamesAndLastName(string raw, string lastName, string[] givenNames)
    {
        var result = parser.Parse(raw);

        result.Name.Should().NotBeNull();
        result.Name!.LastName.Should().Be(lastName);
        result.Name.GivenNames.Should().Equal(givenNames);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_BlankInput_IsIgnored(string raw)
    {
        var result = parser.Parse(raw);

        result.IsIgnored.Should().BeTrue();
    }

    [Fact]
    public void Parse_ValidName_SetsValidStateAndNotIgnored()
    {
        var result = parser.Parse("Janet Parsons");

        result.IsValid.Should().BeTrue();
        result.IsIgnored.Should().BeFalse();
        result.Error.Should().BeNull();
    }

    [Theory]
    [InlineData("Madonna", 1)]
    [InlineData("One Two Three Four Five", 5)]
    [InlineData("Le Blanc", 1)]                 // a bare particle surname leaves no given name
    [InlineData("Anne Bea Cara Dee Le Blanc", 5)] // particles never count as given names
    public void Parse_PartCountOutOfRange_ReturnsError(string raw, int partCount)
    {
        var result = parser.Parse(raw);

        result.IsValid.Should().BeFalse();
        result.Error.Should().Be($"expected 2-4 parts, got {partCount}");
    }

    [Theory]
    [InlineData("  Janet Parsons  ")]
    [InlineData("Janet   Parsons")]
    [InlineData("Janet\tParsons")]
    [InlineData("Janet\u00A0Parsons")]
    public void Parse_IrregularWhitespace_ReturnsExpectedPersonName(string rawName)
    {
        var result = parser.Parse(rawName);

        result.Name.Should().NotBeNull();
        result.Name!.LastName.Should().Be("Parsons");
        result.Name.GivenNames[0].Should().Be("Janet");
    }

    // French surname particles (de, du, des, le, la, les, saint/st...) are part of the last
    // name, so the name sorts under the particle and the particles do not consume given-name slots.
    [Theory]
    [InlineData("Jean Le Blanc", "Le Blanc", new[] { "Jean" })]
    [InlineData("Louis de la Durantaye", "de la Durantaye", new[] { "Louis" })]
    [InlineData("Marie-Ève Des Rosiers", "Des Rosiers", new[] { "Marie-Ève" })]
    [InlineData("Jean Marc de la Durantaye", "de la Durantaye", new[] { "Jean", "Marc" })]
    [InlineData("Anne Bea Cara Du Pont", "Du Pont", new[] { "Anne", "Bea", "Cara" })]
    [InlineData("Pierre Saint Pierre", "Saint Pierre", new[] { "Pierre" })]
    [InlineData("Marc-André St-Pierre", "St-Pierre", new[] { "Marc-André" })]
    public void Parse_SurnameParticles_AreKeptInLastName(string raw, string lastName, string[] givenNames)
    {
        var result = parser.Parse(raw);

        result.IsValid.Should().BeTrue();
        result.Name!.LastName.Should().Be(lastName);
        result.Name.GivenNames.Should().Equal(givenNames);
    }

    [Fact]
    public void Parse_SurnameParticles_MatchCaseInsensitivelyAndPreserveCasing()
    {
        var result = parser.Parse("jean DE LA durantaye");

        result.IsValid.Should().BeTrue();
        result.Name!.LastName.Should().Be("DE LA durantaye");
        result.Name.GivenNames.Should().Equal("jean");
        result.Name.ToString().Should().Be("jean DE LA durantaye");
    }
}
