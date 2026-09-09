using AwesomeAssertions;

namespace NameSorter.Tests;

public class ProgramTests
{
    // This is the end-to-end happy-path test for the CLI: it creates a temporary file,
    // runs the app against it, and then asserts that the output, warnings, exit code,
    // and saved file all reflect the expected sorting and validation behavior.
    [Fact]
    public void Main_ValidFile_SortsWritesAndReportsInvalidLines()
    {
        lock (TestProcessState.Lock)
        {
            using var workingDirectory = new TempWorkingDirectory();
            using var console = new ConsoleCapture();

            // The input includes one valid name, one invalid name, and one additional valid name.
            var inputPath = Path.Combine(workingDirectory.Path, "input.txt");
            File.WriteAllLines(inputPath, ["Janet Parsons", "InvalidSingleName", "Marin Alvarez"]);

            var exitCode = global::NameSorter.Program.Main([inputPath]);

            // Successful execution should return a zero exit code and print names in lastname order.
            exitCode.Should().Be(0);
            console.Out.Should().Be($"Marin Alvarez{Environment.NewLine}Janet Parsons{Environment.NewLine}");

            // The invalid line should be surfaced to stderr without failing the overall process.
            console.Error.Should().Contain("Warning: Line 2");
            console.Error.Should().Contain("Processed 2 valid name(s); skipped 1 invalid line(s).");

            // The application should write the sorted list to the default output file as well.
            File.ReadAllLines(Path.Combine(workingDirectory.Path, "sorted-names-list.txt"))
                .Should().Equal(["Marin Alvarez", "Janet Parsons"]);
        }
    }

    // Blank and whitespace-only lines must not shift the reported line numbers: a warning
    // should cite the name's actual 1-based position in the file.
    [Fact]
    public void Main_BlankLinesBeforeInvalidLine_ReportsActualFileLineNumber()
    {
        lock (TestProcessState.Lock)
        {
            using var workingDirectory = new TempWorkingDirectory();
            using var console = new ConsoleCapture();

            var inputPath = Path.Combine(workingDirectory.Path, "input.txt");
            File.WriteAllLines(
                inputPath,
                ["Janet Parsons", "", "   ", "InvalidSingleName", "Marin Alvarez"]);

            var exitCode = global::NameSorter.Program.Main([inputPath]);

            exitCode.Should().Be(0);
            console.Error.Should().Contain("Warning: Line 4 (\"InvalidSingleName\")");
            console.Error.Should().Contain("Processed 2 valid name(s); skipped 1 invalid line(s).");
        }
    }

    // This test covers the CLI contract when the user does not provide required arguments.
    // The app should fail fast and tell the user how to invoke it correctly.
    [Fact]
    public void Main_MissingArguments_ReturnsInvalidArgumentsExitCode()
    {
        lock (TestProcessState.Lock)
        {
            using var console = new ConsoleCapture();

            var exitCode = global::NameSorter.Program.Main([]);

            exitCode.Should().Be(1);
            console.Error.Should().Contain("Usage: name-sorter <file-path>");
        }
    }

    // This test ensures missing input files are treated as a command-line error with a specific
    // exit code and a clear error message, rather than crashing unexpectedly.
    [Fact]
    public void Main_MissingFile_ReturnsInputFileExitCode()
    {
        lock (TestProcessState.Lock)
        {
            using var console = new ConsoleCapture();

            var exitCode = global::NameSorter.Program.Main(["missing.txt"]);

            exitCode.Should().Be(2);
            console.Error.Should().Contain("Error: File not found: missing.txt");
        }
    }

    // This test covers the unexpected runtime error path. When the application throws while
    // processing a valid input file, the CLI should surface the underlying message and return
    // a dedicated process-level error code instead of crashing unexpectedly. A directory sitting
    // where the output file is expected forces FileNameWriter to throw, portably.
    [Fact]
    public void Main_UnexpectedException_ReturnsApplicationErrorExitCode()
    {
        lock (TestProcessState.Lock)
        {
            using var workingDirectory = new TempWorkingDirectory();
            using var console = new ConsoleCapture();

            var inputPath = Path.Combine(workingDirectory.Path, "input.txt");
            File.WriteAllText(inputPath, "Jane Doe");
            Directory.CreateDirectory(Path.Combine(workingDirectory.Path, "sorted-names-list.txt"));

            var exitCode = global::NameSorter.Program.Main([inputPath]);

            exitCode.Should().Be(3);
            console.Error.Should().Contain("Error:");
        }
    }
}
