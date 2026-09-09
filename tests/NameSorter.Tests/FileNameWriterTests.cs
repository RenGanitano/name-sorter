using AwesomeAssertions;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter.Tests;

public class FileNameWriterTests
{
    [Fact]
    public void Write_ValidNames_WritesToFile()
    {
        lock (TestProcessState.Lock)
        {
            using var workingDirectory = new TempWorkingDirectory();
            var names = new List<PersonName>
            {
                new(["Marin"], "Alvarez"),
                new(["Janet"], "Parsons"),
            };

            new FileNameWriter().Write(names);

            File.ReadAllLines(Path.Combine(workingDirectory.Path, "sorted-names-list.txt"))
                .Should().Equal(["Marin Alvarez", "Janet Parsons"]);
        }
    }

    [Fact]
    public void Write_EmptyList_CreatesEmptyFile()
    {
        lock (TestProcessState.Lock)
        {
            using var workingDirectory = new TempWorkingDirectory();

            new FileNameWriter().Write([]);

            var outputPath = Path.Combine(workingDirectory.Path, "sorted-names-list.txt");
            File.Exists(outputPath).Should().BeTrue();
            File.ReadAllLines(outputPath).Should().BeEmpty();
        }
    }
}
