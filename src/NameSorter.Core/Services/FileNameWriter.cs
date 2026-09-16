using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// Writes sorted names to the given file path, one per line.
/// </summary>
public class FileNameWriter : INameWriter
{
    public const string DefaultOutputFileName = "sorted-names-list.txt";

    private readonly string outputPath;

    public FileNameWriter(string outputPath)
    {
        this.outputPath = outputPath ?? throw new ArgumentNullException(nameof(outputPath));
    }

    public void Write(IList<PersonName> names)
    {
        var lines = names.Select(n => n.ToString()).ToArray();
        File.WriteAllLines(this.outputPath, lines);
    }
}
