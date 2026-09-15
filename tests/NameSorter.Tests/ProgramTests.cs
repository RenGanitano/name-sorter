using AwesomeAssertions;

namespace NameSorter.Tests;

public class ProgramTests
{
    // This is the end-to-end happy-path test for the CLI: it creates a temporary file,
    // runs the app against it, and then asserts that the output, warnings, exit code,
    // and saved file all reflect the expected sorting and validation behavior.
    [Fact]
    public void Run_ValidFile_SortsWritesAndReportsInvalidLines()
    {
        using var directory = new TempDirectory();
        using var output = new StringWriter();
        using var error = new StringWriter();

        // The input includes one valid name, one invalid name, and one additional valid name.
        var inputPath = directory.Combine("input.txt");
        File.WriteAllLines(inputPath, ["Janet Parsons", "InvalidSingleName", "Marin Alvarez"]);

        var exitCode = global::NameSorter.Program.Run([inputPath], output, error, directory.Path);

        // Successful execution should return a zero exit code and print names in lastname order.
        exitCode.Should().Be(0);
        output.ToString().Should().Be($"Marin Alvarez{Environment.NewLine}Janet Parsons{Environment.NewLine}");

        // The invalid line should be surfaced to stderr without failing the overall process.
        error.ToString().Should().Contain("Warning: Line 2");
        error.ToString().Should().Contain("Processed 2 valid name(s); skipped 1 invalid line(s).");

        // The application should write the sorted list to the default output file as well.
        File.ReadAllLines(directory.Combine("sorted-names-list.txt"))
            .Should().Equal(["Marin Alvarez", "Janet Parsons"]);
    }

    // Blank and whitespace-only lines must not shift the reported line numbers: a warning
    // should cite the name's actual 1-based position in the file.
    [Fact]
    public void Run_BlankLinesBeforeInvalidLine_ReportsActualFileLineNumber()
    {
        using var directory = new TempDirectory();
        using var output = new StringWriter();
        using var error = new StringWriter();

        var inputPath = directory.Combine("input.txt");
        File.WriteAllLines(
            inputPath,
            ["Janet Parsons", "", "   ", "InvalidSingleName", "Marin Alvarez"]);

        var exitCode = global::NameSorter.Program.Run([inputPath], output, error, directory.Path);

        exitCode.Should().Be(0);
        error.ToString().Should().Contain("Warning: Line 4 (\"InvalidSingleName\")");
        error.ToString().Should().Contain("Processed 2 valid name(s); skipped 1 invalid line(s).");
    }

    // This test covers the CLI contract when the user does not provide required arguments.
    // The app should fail fast and tell the user how to invoke it correctly.
    [Fact]
    public void Run_MissingArguments_ReturnsInvalidArgumentsExitCode()
    {
        using var output = new StringWriter();
        using var error = new StringWriter();

        var exitCode = global::NameSorter.Program.Run([], output, error, Path.GetTempPath());

        exitCode.Should().Be(1);
        error.ToString().Should().Contain("Usage: name-sorter <file-path>");
    }

    // This test ensures missing input files are treated as a command-line error with a specific
    // exit code and a clear error message, rather than crashing unexpectedly.
    [Fact]
    public void Run_MissingFile_ReturnsInputFileExitCode()
    {
        using var output = new StringWriter();
        using var error = new StringWriter();

        var exitCode = global::NameSorter.Program.Run(["missing.txt"], output, error, Path.GetTempPath());

        exitCode.Should().Be(2);
        error.ToString().Should().Contain("Error: File not found: missing.txt");
    }

    // This test covers the unexpected runtime error path. When the application throws while
    // processing a valid input file, the CLI should surface the underlying message and return
    // a dedicated process-level error code instead of crashing unexpectedly. A directory sitting
    // where the output file is expected forces FileNameWriter to throw, portably.
    [Fact]
    public void Run_UnexpectedException_ReturnsApplicationErrorExitCode()
    {
        using var directory = new TempDirectory();
        using var output = new StringWriter();
        using var error = new StringWriter();

        var inputPath = directory.Combine("input.txt");
        File.WriteAllText(inputPath, "Jane Doe");
        Directory.CreateDirectory(directory.Combine("sorted-names-list.txt"));

        var exitCode = global::NameSorter.Program.Run([inputPath], output, error, directory.Path);

        exitCode.Should().Be(3);
        error.ToString().Should().Contain("Error:");
    }
}
