namespace NameSorter.Services;

/// <summary>
/// Reads raw name strings from a source.
/// </summary>
public interface INameReader
{
    IList<string> Read(string source);
}
