using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter;

/// <summary>
/// Orchestrates the CLI workflow: read → process (parse + sort) → write.
/// All dependencies are constructor-injected.
/// </summary>
public class NameSortingApplication
{
    private readonly INameReader reader;
    private readonly INameSortingProcessor processor;
    private readonly IEnumerable<INameWriter> writers;

    public NameSortingApplication(
        INameReader reader,
        INameSortingProcessor processor,
        IEnumerable<INameWriter> writers)
    {
        this.reader = reader;
        this.processor = processor;
        this.writers = writers;
    }

    /// <summary>
    /// Runs the full name-sorting workflow for the given file path.
    /// </summary>
    public NameSortingResult Run(string filePath, SortDirection direction = SortDirection.Ascending)
    {
        var rawNames = this.reader.Read(filePath);

        var result = this.processor.Process(rawNames, direction);

        foreach (var writer in this.writers)
        {
            writer.Write(result.SortedNames);
        }

        return result;
    }
}
