using NameSorter.Models;

namespace NameSorter;

/// <summary>
/// Parsed command-line arguments: <c>name-sorter [--descending|-d] &lt;file-path&gt;</c>.
/// </summary>
public sealed record CommandLineOptions(string FilePath, SortDirection Direction)
{
    public const string Usage = "Usage: name-sorter [--descending|-d] <file-path>";

    /// <summary>
    /// Parses <paramref name="args"/>. Options may appear in any position; exactly one
    /// positional argument (the input file path) is required.
    /// </summary>
    public static CommandLineParseResult Parse(string[] args)
    {
        string? filePath = null;
        var direction = SortDirection.Ascending;

        foreach (var arg in args)
        {
            switch (arg)
            {
                case "--descending":
                case "-d":
                    direction = SortDirection.Descending;
                    break;

                default:
                    if (arg.StartsWith('-'))
                        return new CommandLineParseResult(null, $"Unknown option: {arg}");

                    if (filePath is not null)
                        return new CommandLineParseResult(null, $"Unexpected argument: {arg}");

                    filePath = arg;
                    break;
            }
        }

        if (filePath is null)
            return new CommandLineParseResult(null, "Missing <file-path>");

        return new CommandLineParseResult(new CommandLineOptions(filePath, direction), null);
    }
}

/// <summary>
/// Represents either successfully parsed options or an argument error.
/// </summary>
public sealed record CommandLineParseResult(CommandLineOptions? Options, string? Error)
{
    public bool IsValid => Options is not null;
}
