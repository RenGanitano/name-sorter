namespace NameSorter.Tests;

/// <summary>
/// Redirects <see cref="Console.Out"/> and <see cref="Console.Error"/> to in-memory
/// buffers for the lifetime of the instance, restoring the originals on dispose.
/// Tests that use this must hold <see cref="TestProcessState.Lock"/>.
/// </summary>
internal sealed class ConsoleCapture : IDisposable
{
    private readonly TextWriter originalOut = Console.Out;
    private readonly TextWriter originalError = Console.Error;
    private readonly StringWriter outWriter = new();
    private readonly StringWriter errorWriter = new();

    public ConsoleCapture()
    {
        Console.SetOut(outWriter);
        Console.SetError(errorWriter);
    }

    public string Out => outWriter.ToString();

    public string Error => errorWriter.ToString();

    public void Dispose()
    {
        Console.SetOut(originalOut);
        Console.SetError(originalError);
        outWriter.Dispose();
        errorWriter.Dispose();
    }
}

/// <summary>
/// Creates a unique temporary directory and switches the process working directory to
/// it, restoring the previous working directory and deleting the temp tree on dispose.
/// Tests that use this must hold <see cref="TestProcessState.Lock"/>.
/// </summary>
internal sealed class TempWorkingDirectory : IDisposable
{
    private readonly string originalDirectory = Directory.GetCurrentDirectory();

    public TempWorkingDirectory()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"name-sorter-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path);
        Directory.SetCurrentDirectory(Path);
    }

    public string Path { get; }

    public void Dispose()
    {
        Directory.SetCurrentDirectory(originalDirectory);
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
