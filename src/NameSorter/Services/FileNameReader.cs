namespace NameSorter.Services;

/// <summary>
/// Reads names from a file, one name per line. Every line is returned, including blank
/// ones, so downstream processing can report accurate 1-based file line numbers.
/// </summary>
public class FileNameReader : INameReader
{
    public IList<string> Read(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}", filePath);

        return File.ReadAllLines(filePath).ToList();
    }
}
