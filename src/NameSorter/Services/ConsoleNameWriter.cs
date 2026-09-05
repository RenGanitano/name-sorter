using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// Writes sorted names to the console (stdout).
/// </summary>
public class ConsoleNameWriter : INameWriter
{
    public void Write(IList<PersonName> names)
    {
        foreach (var name in names)
        {
            Console.WriteLine(name.ToString());
        }
    }
}
