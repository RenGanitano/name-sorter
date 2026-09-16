using AwesomeAssertions;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter.Tests;

public class ConsoleNameWriterTests
{
    [Fact]
    public void Write_ValidNames_OutputsOneNamePerLine()
    {
        using var output = new StringWriter();
        var names = new List<PersonName>
        {
            new(["Marin"], "Alvarez"),
            new(["Janet"], "Parsons"),
        };

        new ConsoleNameWriter(output).Write(names);

        output.ToString().Should().Be($"Marin Alvarez{Environment.NewLine}Janet Parsons{Environment.NewLine}");
    }

    [Fact]
    public void Constructor_NullOutput_ThrowsArgumentNullException()
    {
        Action act = () => new ConsoleNameWriter(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("output");
    }
}
