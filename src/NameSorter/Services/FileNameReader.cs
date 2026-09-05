namespace NameSorter.Services;

/// <summary>
/// Reads names from a file, one name per line.
/// </summary>
public class FileNameReader : INameReader
{
    public IList<string> Read(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}", filePath);

        return File.ReadAllLines(filePath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();
    }
}
