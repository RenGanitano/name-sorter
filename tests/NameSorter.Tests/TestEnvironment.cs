namespace NameSorter.Tests;

/// <summary>
/// Creates a unique temporary directory and deletes the whole tree on dispose.
/// The process working directory is never changed, so tests using this stay parallel-safe.
/// </summary>
internal sealed class TempDirectory : IDisposable
{
    public TempDirectory()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"name-sorter-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path);
    }

    public string Path { get; }

    public string Combine(string fileName) => System.IO.Path.Combine(Path, fileName);

    public void Dispose()
    {
        try
        {
            Directory.Delete(Path, recursive: true);
        }
        catch (IOException)
        {
            // Best-effort cleanup; a leaked temp directory should not fail a test.
        }
    }
}
