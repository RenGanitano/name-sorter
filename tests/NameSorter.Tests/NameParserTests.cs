using AwesomeAssertions;
using NameSorter.Services;

namespace NameSorter.Tests;

public class NameParserTests
{
    private readonly NameParser parser = new();

    [Fact]
    public void Parse_OneGivenName_ReturnsExpectedPersonName()
    {
        var result = parser.Parse("Janet Parsons");

        result.Name.Should().NotBeNull();
        result.Name!.LastName.Should().Be("Parsons");
        result.Name.GivenNames.Should().ContainSingle();
        result.Name.GivenNames[0].Should().Be("Janet");
    }

    [Fact]
    public void Parse_TwoGivenNames_ReturnsExpectedPersonName()
    {
        var result = parser.Parse("Adonis Julius Archer");

        result.Name.Should().NotBeNull();
        result.Name!.LastName.Should().Be("Archer");
        result.Name.GivenNames.Should().HaveCount(2);
        result.Name.GivenNames[0].Should().Be("Adonis");
        result.Name.GivenNames[1].Should().Be("Julius");
    }

    [Fact]
    public void Parse_ThreeGivenNames_ReturnsExpectedPersonName()
    {
        var result = parser.Parse("Hunter Uriah Mathew Clarke");

        result.Name.Should().NotBeNull();
        result.Name!.LastName.Should().Be("Clarke");
        result.Name.GivenNames.Should().HaveCount(3);
        result.Name.GivenNames[0].Should().Be("Hunter");
        result.Name.GivenNames[1].Should().Be("Uriah");
        result.Name.GivenNames[2].Should().Be("Mathew");
    }

    [Fact]
    public void Parse_EmptyString_ReturnsNull()
    {
        var result = parser.Parse("");

        result.IsIgnored.Should().BeTrue();
    }

    [Fact]
    public void Parse_WhitespaceOnly_ReturnsNull()
    {
        var result = parser.Parse("   ");

        result.IsIgnored.Should().BeTrue();
    }

    [Fact]
    public void Parse_SingleWord_ReturnsNull()
    {
        var result = parser.Parse("Madonna");

        result.IsValid.Should().BeFalse();
        result.Error.Should().Be("expected 2-4 parts, got 1");
    }

    [Fact]
    public void Parse_FiveWords_ReturnsNull()
    {
        var result = parser.Parse("One Two Three Four Five");

        result.IsValid.Should().BeFalse();
        result.Error.Should().Be("expected 2-4 parts, got 5");
    }

    [Fact]
    public void Parse_LeadingAndTrailingWhitespace_ReturnsExpectedPersonName()
    {
        var result = parser.Parse("  Janet Parsons  ");

        result.Name.Should().NotBeNull();
        result.Name!.LastName.Should().Be("Parsons");
        result.Name.GivenNames[0].Should().Be("Janet");
    }

    [Fact]
    public void Parse_MultipleSpacesBetweenNames_ReturnsExpectedPersonName()
    {
        var result = parser.Parse("Janet   Parsons");

        result.Name.Should().NotBeNull();
        result.Name!.LastName.Should().Be("Parsons");
        result.Name.GivenNames[0].Should().Be("Janet");
    }

    [Theory]
    [InlineData("Janet\tParsons")]
    [InlineData("Janet\u00A0Parsons")]
    public void Parse_TabAndNonBreakingSpaceBetweenNames_ReturnsExpectedPersonName(string rawName)
    {
        var result = parser.Parse(rawName);

        result.Name.Should().NotBeNull();
        result.Name!.LastName.Should().Be("Parsons");
        result.Name.GivenNames[0].Should().Be("Janet");
    }
}
