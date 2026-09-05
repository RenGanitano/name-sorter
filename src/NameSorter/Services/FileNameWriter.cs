using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// Writes sorted names to a file called "sorted-names-list.txt" in the working directory.
/// </summary>
public class FileNameWriter : INameWriter
{
    private const string OutputFileName = "sorted-names-list.txt";

    public void Write(IList<PersonName> names)
    {
        var lines = names.Select(n => n.ToString()).ToArray();
        File.WriteAllLines(OutputFileName, lines);
    }
}
