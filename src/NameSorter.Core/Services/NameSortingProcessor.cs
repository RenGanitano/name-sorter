using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// The source-agnostic core of the workflow: parse → sort. Readers and writers live
/// with the host (CLI, API) that owns the I/O.
/// </summary>
public class NameSortingProcessor : INameSortingProcessor
{
    private readonly INameParser parser;
    private readonly INameSorter sorter;

    public NameSortingProcessor(INameParser parser, INameSorter sorter)
    {
        this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
        this.sorter = sorter ?? throw new ArgumentNullException(nameof(sorter));
    }

    public NameSortingResult Process(IList<string> rawNames, SortDirection direction = SortDirection.Ascending)
    {
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

        var sortedNames = this.sorter.Sort(parsedNames, direction);

        return new NameSortingResult(sortedNames, invalidNames);
    }
}
