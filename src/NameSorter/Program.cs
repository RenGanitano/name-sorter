using Microsoft.Extensions.DependencyInjection;
using NameSorter.Comparers;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: name-sorter <file-path>");
            return;
        }

        var filePath = args[0];

        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"Error: File not found — {filePath}");
            return;
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
            app.Run(filePath);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
        }
    }
}
