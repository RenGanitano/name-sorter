using AwesomeAssertions;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter.Tests;

public class FileNameWriterTests
{
    [Fact]
    public void Write_ValidNames_WritesToFile()
    {
        using var directory = new TempDirectory();
        var outputPath = directory.Combine("sorted-names-list.txt");
        var names = new List<PersonName>
        {
            new(["Marin"], "Alvarez"),
            new(["Janet"], "Parsons"),
        };

        new FileNameWriter(outputPath).Write(names);

        File.ReadAllLines(outputPath).Should().Equal(["Marin Alvarez", "Janet Parsons"]);
    }

    [Fact]
    public void Write_EmptyList_CreatesEmptyFile()
    {
        using var directory = new TempDirectory();
        var outputPath = directory.Combine("sorted-names-list.txt");

        new FileNameWriter(outputPath).Write([]);

        File.Exists(outputPath).Should().BeTrue();
        File.ReadAllLines(outputPath).Should().BeEmpty();
    }

    [Fact]
    public void Constructor_NullOutputPath_ThrowsArgumentNullException()
    {
        Action act = () => new FileNameWriter(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("outputPath");
    }
}
