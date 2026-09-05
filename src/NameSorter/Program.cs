using Microsoft.Extensions.DependencyInjection;
using NameSorter.Comparers;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: name-sorter <file-path>");
            return 1;
        }

        var filePath = args[0];

        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"Error: File not found: {filePath}");
            return 2;
        }

        // Configure DI container
        var services = new ServiceCollection();
        services.AddSingleton<INameReader, FileNameReader>();
        services.AddSingleton<INameParser, NameParser>();
        services.AddSingleton<IComparer<PersonName>, LastNameFirstComparer>();
        services.AddSingleton<INameSorter, NameSorterService>();
        services.AddSingleton<INameWriter, ConsoleNameWriter>();
        services.AddSingleton<INameWriter, FileNameWriter>();
        services.AddSingleton<NameSortingApplication>();

        var provider = services.BuildServiceProvider();
        var app = provider.GetRequiredService<NameSortingApplication>();

        try
        {
            var result = app.Run(filePath);

            foreach (var invalidName in result.InvalidNames)
            {
                Console.Error.WriteLine(
                    $"Warning: Line {invalidName.LineNumber} (\"{invalidName.Value}\") skipped: {invalidName.Reason}.");
            }

            Console.Error.WriteLine(
                $"Processed {result.SortedNames.Count} valid name(s); skipped {result.InvalidNames.Count} invalid line(s).");

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 3;
        }
    }
}
