using AwesomeAssertions;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter.Tests;

public class ConsoleNameWriterTests
{
    [Fact]
    public void Write_ValidNames_OutputsToConsole()
    {
        lock (TestProcessState.Lock)
        {
            using var console = new ConsoleCapture();
            var names = new List<PersonName>
            {
                new(["Marin"], "Alvarez"),
                new(["Janet"], "Parsons"),
            };

            new ConsoleNameWriter().Write(names);

            console.Out.Should().Be($"Marin Alvarez{Environment.NewLine}Janet Parsons{Environment.NewLine}");
        }
    }
}
