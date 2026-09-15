using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using NameSorter.Comparers;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter;

public class Program
{
    // Thin shim binding the real console and working directory; all logic is in Run.
    [ExcludeFromCodeCoverage]
    public static int Main(string[] args)
    {
        return Run(args, Console.Out, Console.Error, Directory.GetCurrentDirectory());
    }

    /// <summary>
    /// Runs the CLI with explicit output streams and output directory so it can be
    /// exercised without touching process-global state.
    /// </summary>
    public static int Run(string[] args, TextWriter output, TextWriter error, string outputDirectory)
    {
        if (args.Length == 0)
        {
            error.WriteLine("Usage: name-sorter <file-path>");
            return 1;
        }

        var filePath = args[0];

        if (!File.Exists(filePath))
        {
            error.WriteLine($"Error: File not found: {filePath}");
            return 2;
        }

        var outputPath = Path.Combine(outputDirectory, FileNameWriter.DefaultOutputFileName);

        // Configure DI container
        var services = new ServiceCollection();
        services.AddSingleton<INameReader, FileNameReader>();
        services.AddSingleton<INameParser, NameParser>();
        services.AddSingleton<IComparer<PersonName>, LastNameFirstComparer>();
        services.AddSingleton<INameSorter, NameSorterService>();
        services.AddSingleton<INameWriter>(new ConsoleNameWriter(output));
        services.AddSingleton<INameWriter>(new FileNameWriter(outputPath));
        services.AddSingleton<NameSortingApplication>();

        var provider = services.BuildServiceProvider();
        var app = provider.GetRequiredService<NameSortingApplication>();

        try
        {
            var result = app.Run(filePath);

            foreach (var invalidName in result.InvalidNames)
            {
                error.WriteLine(
                    $"Warning: Line {invalidName.LineNumber} (\"{invalidName.Value}\") skipped: {invalidName.Reason}.");
            }

            error.WriteLine(
                $"Processed {result.SortedNames.Count} valid name(s); skipped {result.InvalidNames.Count} invalid line(s).");

            return 0;
        }
        catch (Exception ex)
        {
            error.WriteLine($"Error: {ex.Message}");
            return 3;
        }
    }
}
