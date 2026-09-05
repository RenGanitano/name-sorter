using AwesomeAssertions;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter.Tests;

public class FileNameWriterTests
{
    private static readonly object CurrentDirectoryLock = new();

    [Fact]
    public void Write_ValidNames_WritesToFile()
    {
        lock (CurrentDirectoryLock)
        {
            var originalDirectory = Directory.GetCurrentDirectory();
            var directory = Path.Combine(Path.GetTempPath(), $"name-sorter-{Guid.NewGuid():N}");
            Directory.CreateDirectory(directory);

            try
            {
                Directory.SetCurrentDirectory(directory);
                var names = new List<PersonName>
                {
                    new(["Marin"], "Alvarez"),
                    new(["Janet"], "Parsons"),
                };

                new FileNameWriter().Write(names);

                File.ReadAllLines(Path.Combine(directory, "sorted-names-list.txt"))
                    .Should().Equal(["Marin Alvarez", "Janet Parsons"]);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    [Fact]
    public void Write_EmptyList_CreatesEmptyFile()
    {
        lock (CurrentDirectoryLock)
        {
            var originalDirectory = Directory.GetCurrentDirectory();
            var directory = Path.Combine(Path.GetTempPath(), $"name-sorter-{Guid.NewGuid():N}");
            Directory.CreateDirectory(directory);

            try
            {
                Directory.SetCurrentDirectory(directory);

                new FileNameWriter().Write([]);

                var outputPath = Path.Combine(directory, "sorted-names-list.txt");
                File.Exists(outputPath).Should().BeTrue();
                File.ReadAllLines(outputPath).Should().BeEmpty();
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
                Directory.Delete(directory, recursive: true);
            }
        }
    }
}