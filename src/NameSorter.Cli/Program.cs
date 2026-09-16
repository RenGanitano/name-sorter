using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using NameSorter.DependencyInjection;
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
        var parsedArgs = CommandLineOptions.Parse(args);
        if (!parsedArgs.IsValid)
        {
            error.WriteLine($"Error: {parsedArgs.Error}");
            error.WriteLine(CommandLineOptions.Usage);
            return 1;
        }

        var options = parsedArgs.Options!;
        var filePath = options.FilePath;

        if (!File.Exists(filePath))
        {
            error.WriteLine($"Error: File not found: {filePath}");
            return 2;
        }

        var outputPath = Path.Combine(outputDirectory, FileNameWriter.DefaultOutputFileName);

        // Configure DI container: the shared parse → sort pipeline plus the CLI's own I/O.
        var services = new ServiceCollection();
        services.AddNameSorting();
        services.AddSingleton<INameReader, FileNameReader>();
        services.AddSingleton<INameWriter>(new ConsoleNameWriter(output));
        services.AddSingleton<INameWriter>(new FileNameWriter(outputPath));
        services.AddSingleton<NameSortingApplication>();

        var provider = services.BuildServiceProvider();
        var app = provider.GetRequiredService<NameSortingApplication>();

        try
        {
            var result = app.Run(filePath, options.Direction);

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
