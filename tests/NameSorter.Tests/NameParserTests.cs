using NameSorter.Services;

namespace NameSorter.Tests;

public class NameParserTests
{
    private readonly NameParser _parser = new();

    [Fact]
    public void Parse_OneGivenName_ReturnsExpectedPersonName()
    {
        var result = _parser.Parse("Janet Parsons");

        Assert.NotNull(result);
        Assert.Equal("Parsons", result.LastName);
        Assert.Single(result.GivenNames);
        Assert.Equal("Janet", result.GivenNames[0]);
    }

    [Fact]
    public void Parse_TwoGivenNames_ReturnsExpectedPersonName()
    {
        var result = _parser.Parse("Adonis Julius Archer");

        Assert.NotNull(result);
        Assert.Equal("Archer", result.LastName);
        Assert.Equal(2, result.GivenNames.Count);
        Assert.Equal("Adonis", result.GivenNames[0]);
        Assert.Equal("Julius", result.GivenNames[1]);
    }

    [Fact]
    public void Parse_ThreeGivenNames_ReturnsExpectedPersonName()
    {
        var result = _parser.Parse("Hunter Uriah Mathew Clarke");

        Assert.NotNull(result);
        Assert.Equal("Clarke", result.LastName);
        Assert.Equal(3, result.GivenNames.Count);
        Assert.Equal("Hunter", result.GivenNames[0]);
        Assert.Equal("Uriah", result.GivenNames[1]);
        Assert.Equal("Mathew", result.GivenNames[2]);
    }

    [Fact]
    public void Parse_EmptyString_ReturnsNull()
    {
        var result = _parser.Parse("");

        Assert.Null(result);
    }

    [Fact]
    public void Parse_WhitespaceOnly_ReturnsNull()
    {
        var result = _parser.Parse("   ");

        Assert.Null(result);
    }

    [Fact]
    public void Parse_SingleWord_ReturnsNull()
    {
        var result = _parser.Parse("Madonna");

        Assert.Null(result);
    }

    [Fact]
    public void Parse_FiveWords_ReturnsNull()
    {
        var result = _parser.Parse("One Two Three Four Five");

        Assert.Null(result);
    }

    [Fact]
    public void Parse_LeadingAndTrailingWhitespace_ReturnsExpectedPersonName()
    {
        var result = _parser.Parse("  Janet Parsons  ");

        Assert.NotNull(result);
        Assert.Equal("Parsons", result.LastName);
        Assert.Equal("Janet", result.GivenNames[0]);
    }

    [Fact]
    public void Parse_MultipleSpacesBetweenNames_ReturnsExpectedPersonName()
    {
        var result = _parser.Parse("Janet   Parsons");

        Assert.NotNull(result);
        Assert.Equal("Parsons", result.LastName);
        Assert.Equal("Janet", result.GivenNames[0]);
    }

    [Theory]
    [InlineData("Janet\tParsons")]
    [InlineData("Janet\u00A0Parsons")]
    public void Parse_TabAndNonBreakingSpaceBetweenNames_ReturnsExpectedPersonName(string rawName)
    {
        var result = _parser.Parse(rawName);

        Assert.NotNull(result);
        Assert.Equal("Parsons", result.LastName);
        Assert.Equal("Janet", result.GivenNames[0]);
    }
}
