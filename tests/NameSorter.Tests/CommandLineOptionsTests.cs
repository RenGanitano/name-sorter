using AwesomeAssertions;
using NameSorter.Models;

namespace NameSorter.Tests;

public class CommandLineOptionsTests
{
    [Fact]
    public void Parse_FilePathOnly_DefaultsToAscending()
    {
        var result = CommandLineOptions.Parse(["names.txt"]);

        result.IsValid.Should().BeTrue();
        result.Error.Should().BeNull();
        result.Options.Should().Be(new CommandLineOptions("names.txt", SortDirection.Ascending));
    }

    [Theory]
    [InlineData("--descending", "names.txt")]
    [InlineData("-d", "names.txt")]
    [InlineData("names.txt", "--descending")]
    [InlineData("names.txt", "-d")]
    public void Parse_DescendingFlagInAnyPosition_SetsDescending(string first, string second)
    {
        var result = CommandLineOptions.Parse([first, second]);

        result.IsValid.Should().BeTrue();
        result.Options.Should().Be(new CommandLineOptions("names.txt", SortDirection.Descending));
    }

    [Fact]
    public void Parse_NoArguments_ReturnsMissingFilePathError()
    {
        var result = CommandLineOptions.Parse([]);

        result.IsValid.Should().BeFalse();
        result.Options.Should().BeNull();
        result.Error.Should().Be("Missing <file-path>");
    }

    [Fact]
    public void Parse_OnlyFlag_ReturnsMissingFilePathError()
    {
        var result = CommandLineOptions.Parse(["-d"]);

        result.IsValid.Should().BeFalse();
        result.Error.Should().Be("Missing <file-path>");
    }

    [Fact]
    public void Parse_UnknownOption_ReturnsError()
    {
        var result = CommandLineOptions.Parse(["--bogus", "names.txt"]);

        result.IsValid.Should().BeFalse();
        result.Error.Should().Be("Unknown option: --bogus");
    }

    [Fact]
    public void Parse_SecondPositionalArgument_ReturnsError()
    {
        var result = CommandLineOptions.Parse(["names.txt", "other.txt"]);

        result.IsValid.Should().BeFalse();
        result.Error.Should().Be("Unexpected argument: other.txt");
    }

    [Fact]
    public void Usage_DescribesFlagAndFilePath()
    {
        CommandLineOptions.Usage.Should().Be("Usage: name-sorter [--descending|-d] <file-path>");
    }
}
