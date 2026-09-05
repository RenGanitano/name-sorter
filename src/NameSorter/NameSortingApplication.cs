using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter;

/// <summary>
/// Orchestrates the name sorting workflow: read → parse → sort → write.
/// All dependencies are constructor-injected.
/// </summary>
public class NameSortingApplication
{
    private readonly INameReader reader;
    private readonly INameParser parser;
    private readonly INameSorter sorter;
    private readonly IEnumerable<INameWriter> writers;

    public NameSortingApplication(
        INameReader reader,
        INameParser parser,
        INameSorter sorter,
        IEnumerable<INameWriter> writers)
    {
        this.reader = reader;
        this.parser = parser;
        this.sorter = sorter;
        this.writers = writers;
    }

    /// <summary>
    /// Runs the full name-sorting workflow for the given file path.
    /// </summary>
    public NameSortingResult Run(string filePath)
    {
        var rawNames = this.reader.Read(filePath);

        var parsedNames = new List<PersonName>();
        var invalidNames = new List<InvalidName>();

        for (var index = 0; index < rawNames.Count; index++)
        {
            var rawName = rawNames[index];
            var parseResult = this.parser.Parse(rawName);
            if (parseResult.IsValid)
            {
                parsedNames.Add(parseResult.Name!);
            }
            else if (!parseResult.IsIgnored)
            {
                invalidNames.Add(new InvalidName(index + 1, rawName, parseResult.Error!));
            }
        }

        var sortedNames = this.sorter.Sort(parsedNames);

        foreach (var writer in this.writers)
        {
            writer.Write(sortedNames);
        }

        return new NameSortingResult(sortedNames, invalidNames);
    }
}
