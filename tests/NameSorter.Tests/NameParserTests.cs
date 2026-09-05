using NameSorter.Services;

namespace NameSorter.Tests;

public class NameParserTests
{
    private readonly NameParser _parser = new();

    [Fact]
    public void Parse_OneGivenName_ReturnsCorrectPersonName()
    {
        var result = _parser.Parse("Janet Parsons");

        Assert.NotNull(result);
        Assert.Equal("Parsons", result.LastName);
        Assert.Single(result.GivenNames);
        Assert.Equal("Janet", result.GivenNames[0]);
    }

    [Fact]
    public void Parse_TwoGivenNames_ReturnsCorrectPersonName()
    {
        var result = _parser.Parse("Adonis Julius Archer");

        Assert.NotNull(result);
        Assert.Equal("Archer", result.LastName);
        Assert.Equal(2, result.GivenNames.Count);
        Assert.Equal("Adonis", result.GivenNames[0]);
        Assert.Equal("Julius", result.GivenNames[1]);
    }

    [Fact]
    public void Parse_ThreeGivenNames_ReturnsCorrectPersonName()
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
        Assert.Null(_parser.Parse(""));
    }

    [Fact]
    public void Parse_WhitespaceOnly_ReturnsNull()
    {
        Assert.Null(_parser.Parse("   "));
    }

    [Fact]
    public void Parse_SingleWord_ReturnsNull()
    {
        Assert.Null(_parser.Parse("Madonna"));
    }

    [Fact]
    public void Parse_FiveWords_ReturnsNull()
    {
        Assert.Null(_parser.Parse("One Two Three Four Five"));
    }

    [Fact]
    public void Parse_TrimsLeadingAndTrailingWhitespace()
    {
        var result = _parser.Parse("  Janet Parsons  ");

        Assert.NotNull(result);
        Assert.Equal("Parsons", result.LastName);
        Assert.Equal("Janet", result.GivenNames[0]);
    }

    [Fact]
    public void Parse_HandlesMultipleSpacesBetweenNames()
    {
        var result = _parser.Parse("Janet   Parsons");

        Assert.NotNull(result);
        Assert.Equal("Parsons", result.LastName);
        Assert.Equal("Janet", result.GivenNames[0]);
    }

    [Theory]
    [InlineData("Janet\tParsons")]
    [InlineData("Janet\u00A0Parsons")]
    public void Parse_HandlesWhitespaceBetweenNames(string rawName)
    {
        var result = _parser.Parse(rawName);

        Assert.NotNull(result);
        Assert.Equal("Parsons", result.LastName);
        Assert.Equal("Janet", result.GivenNames[0]);
    }
}
