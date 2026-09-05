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
            var originalOutput = Console.Out;

            try
            {
                using var output = new StringWriter();
                Console.SetOut(output);
                var names = new List<PersonName>
                {
                    new(["Marin"], "Alvarez"),
                    new(["Janet"], "Parsons"),
                };

                new ConsoleNameWriter().Write(names);

                output.ToString().Should().Be($"Marin Alvarez{Environment.NewLine}Janet Parsons{Environment.NewLine}");
            }
            finally
            {
                Console.SetOut(originalOutput);
            }
        }
    }
}