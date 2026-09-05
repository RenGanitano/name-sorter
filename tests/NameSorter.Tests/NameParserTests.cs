using NameSorter.Services;

namespace NameSorter.Tests;

public class NameParserTests
{
    private readonly NameParser parser = new();

    [Fact]
    public void Parse_OneGivenName_ReturnsExpectedPersonName()
    {
        var result = parser.Parse("Janet Parsons");

        Assert.NotNull(result.Name);
        Assert.Equal("Parsons", result.Name.LastName);
        Assert.Single(result.Name.GivenNames);
        Assert.Equal("Janet", result.Name.GivenNames[0]);
    }

    [Fact]
    public void Parse_TwoGivenNames_ReturnsExpectedPersonName()
    {
        var result = parser.Parse("Adonis Julius Archer");

        Assert.NotNull(result.Name);
        Assert.Equal("Archer", result.Name.LastName);
        Assert.Equal(2, result.Name.GivenNames.Count);
        Assert.Equal("Adonis", result.Name.GivenNames[0]);
        Assert.Equal("Julius", result.Name.GivenNames[1]);
    }

    [Fact]
    public void Parse_ThreeGivenNames_ReturnsExpectedPersonName()
    {
        var result = parser.Parse("Hunter Uriah Mathew Clarke");

        Assert.NotNull(result.Name);
        Assert.Equal("Clarke", result.Name.LastName);
        Assert.Equal(3, result.Name.GivenNames.Count);
        Assert.Equal("Hunter", result.Name.GivenNames[0]);
        Assert.Equal("Uriah", result.Name.GivenNames[1]);
        Assert.Equal("Mathew", result.Name.GivenNames[2]);
    }

    [Fact]
    public void Parse_EmptyString_ReturnsNull()
    {
        var result = parser.Parse("");

        Assert.True(result.IsIgnored);
    }

    [Fact]
    public void Parse_WhitespaceOnly_ReturnsNull()
    {
        var result = parser.Parse("   ");

        Assert.True(result.IsIgnored);
    }

    [Fact]
    public void Parse_SingleWord_ReturnsNull()
    {
        var result = parser.Parse("Madonna");

        Assert.False(result.IsValid);
        Assert.Equal("expected 2-4 parts, got 1", result.Error);
    }

    [Fact]
    public void Parse_FiveWords_ReturnsNull()
    {
        var result = parser.Parse("One Two Three Four Five");

        Assert.False(result.IsValid);
        Assert.Equal("expected 2-4 parts, got 5", result.Error);
    }

    [Fact]
    public void Parse_LeadingAndTrailingWhitespace_ReturnsExpectedPersonName()
    {
        var result = parser.Parse("  Janet Parsons  ");

        Assert.NotNull(result.Name);
        Assert.Equal("Parsons", result.Name.LastName);
        Assert.Equal("Janet", result.Name.GivenNames[0]);
    }

    [Fact]
    public void Parse_MultipleSpacesBetweenNames_ReturnsExpectedPersonName()
    {
        var result = parser.Parse("Janet   Parsons");

        Assert.NotNull(result.Name);
        Assert.Equal("Parsons", result.Name.LastName);
        Assert.Equal("Janet", result.Name.GivenNames[0]);
    }

    [Theory]
    [InlineData("Janet\tParsons")]
    [InlineData("Janet\u00A0Parsons")]
    public void Parse_TabAndNonBreakingSpaceBetweenNames_ReturnsExpectedPersonName(string rawName)
    {
        var result = parser.Parse(rawName);

        Assert.NotNull(result.Name);
        Assert.Equal("Parsons", result.Name.LastName);
        Assert.Equal("Janet", result.Name.GivenNames[0]);
    }
}
