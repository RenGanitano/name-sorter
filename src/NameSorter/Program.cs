using Microsoft.Extensions.DependencyInjection;
using NameSorter.Comparers;
using NameSorter.Models;

namespace NameSorter;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: name-sorter <file-path>");
        }

        var filePath = args[0];

        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"Error: File not found — {filePath}");
        }
    }
}
