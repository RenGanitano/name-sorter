using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// Writes sorted names to a text stream, one per line (stdout in production).
/// </summary>
public class ConsoleNameWriter : INameWriter
{
    private readonly TextWriter output;

    public ConsoleNameWriter(TextWriter output)
    {
        this.output = output ?? throw new ArgumentNullException(nameof(output));
    }

    public void Write(IList<PersonName> names)
    {
        foreach (var name in names)
        {
            this.output.WriteLine(name.ToString());
        }
    }
}
