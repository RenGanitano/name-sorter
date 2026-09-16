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
}
