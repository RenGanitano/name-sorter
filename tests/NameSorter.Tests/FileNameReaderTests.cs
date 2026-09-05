using AwesomeAssertions;
using NameSorter.Services;

namespace NameSorter.Tests;

public class FileNameReaderTests
{
    [Fact]
    public void Read_ExistingFile_ReturnsNonEmptyLinesInOrder()
    {
        var directory = CreateTemporaryDirectory();
        var filePath = Path.Combine(directory, "names.txt");

        try
        {
            File.WriteAllLines(filePath, ["Janet Parsons", "", "   ", "Marin Alvarez"]);

            var names = new FileNameReader().Read(filePath);

            names.Should().Equal(["Janet Parsons", "Marin Alvarez"]);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void Read_NonExistentFile_ThrowsFileNotFoundException()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"missing-{Guid.NewGuid():N}.txt");

        Action act = () => new FileNameReader().Read(filePath);
        var exception = act.Should().Throw<FileNotFoundException>().Which;

        exception.FileName.Should().Be(filePath);
    }

    private static string CreateTemporaryDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"name-sorter-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        return directory;
    }
}