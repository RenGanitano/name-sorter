using AwesomeAssertions;
using NameSorter.Services;

namespace NameSorter.Tests;

public class NameParserTests
{
    private readonly NameParser _parser = new();

    [Fact]
    public void Parse_OneGivenName_ReturnsExpectedPersonName()
    {
        var result = _parser.Parse("Janet Parsons");

        result.Should().NotBeNull();
        result.LastName.Should().Be("Parsons");
        result.GivenNames.Should().ContainSingle();
        result.GivenNames[0].Should().Be("Janet");
    }

    [Fact]
    public void Parse_TwoGivenNames_ReturnsExpectedPersonName()
    {
        var result = _parser.Parse("Adonis Julius Archer");

        result.Should().NotBeNull();
        result.LastName.Should().Be("Archer");
        result.GivenNames.Should().HaveCount(2);
        result.GivenNames[0].Should().Be("Adonis");
        result.GivenNames[1].Should().Be("Julius");
    }

    [Fact]
    public void Parse_ThreeGivenNames_ReturnsExpectedPersonName()
    {
        var result = _parser.Parse("Hunter Uriah Mathew Clarke");

        result.Should().NotBeNull();
        result.LastName.Should().Be("Clarke");
        result.GivenNames.Should().HaveCount(3);
        result.GivenNames[0].Should().Be("Hunter");
        result.GivenNames[1].Should().Be("Uriah");
        result.GivenNames[2].Should().Be("Mathew");
    }

    [Fact]
    public void Parse_EmptyString_ReturnsNull()
    {
        var result = _parser.Parse("");

        result.Should().BeNull();
    }

    [Fact]
    public void Parse_WhitespaceOnly_ReturnsNull()
    {
        var result = _parser.Parse("   ");

        result.Should().BeNull();
    }

    [Fact]
    public void Parse_SingleWord_ReturnsNull()
    {
        var result = _parser.Parse("Madonna");

        result.Should().BeNull();
    }

    [Fact]
    public void Parse_FiveWords_ReturnsNull()
    {
        var result = _parser.Parse("One Two Three Four Five");

        result.Should().BeNull();
    }

    [Fact]
    public void Parse_LeadingAndTrailingWhitespace_ReturnsExpectedPersonName()
    {
        var result = _parser.Parse("  Janet Parsons  ");

        result.Should().NotBeNull();
        result.LastName.Should().Be("Parsons");
        result.GivenNames[0].Should().Be("Janet");
    }

    [Fact]
    public void Parse_MultipleSpacesBetweenNames_ReturnsExpectedPersonName()
    {
        var result = _parser.Parse("Janet   Parsons");

        result.Should().NotBeNull();
        result.LastName.Should().Be("Parsons");
        result.GivenNames[0].Should().Be("Janet");
    }

    [Theory]
    [InlineData("Janet\tParsons")]
    [InlineData("Janet\u00A0Parsons")]
    public void Parse_TabAndNonBreakingSpaceBetweenNames_ReturnsExpectedPersonName(string rawName)
    {
        var result = _parser.Parse(rawName);

        result.Should().NotBeNull();
        result.LastName.Should().Be("Parsons");
        result.GivenNames[0].Should().Be("Janet");
    }
}
