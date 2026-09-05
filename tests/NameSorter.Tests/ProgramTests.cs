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
            // Keep the working directory and console streams isolated so tests do not interfere.
            var originalDirectory = Directory.GetCurrentDirectory();
            var originalOut = Console.Out;
            var originalError = Console.Error;
            var temporaryDirectory = Path.Combine(Path.GetTempPath(), $"name-sorter-{Guid.NewGuid():N}");
            Directory.CreateDirectory(temporaryDirectory);

            try
            {
                // The input includes one valid name, one invalid name, and one additional valid name.
                var inputPath = Path.Combine(temporaryDirectory, "input.txt");
                File.WriteAllLines(inputPath, ["Janet Parsons", "InvalidSingleName", "Marin Alvarez"]);
                Directory.SetCurrentDirectory(temporaryDirectory);

                using var output = new StringWriter();
                using var error = new StringWriter();
                Console.SetOut(output);
                Console.SetError(error);

                var exitCode = global::NameSorter.Program.Main([inputPath]);

                // Successful execution should return a zero exit code and print names in lastname order.
                exitCode.Should().Be(0);
                output.ToString().Should().Be("Marin Alvarez\nJanet Parsons\n");

                // The invalid line should be surfaced to stderr without failing the overall process.
                error.ToString().Should().Contain("Warning: Line 2");
                error.ToString().Should().Contain("Processed 2 valid name(s); skipped 1 invalid line(s).");

                // The application should write the sorted list to the default output file as well.
                File.ReadAllLines(Path.Combine(temporaryDirectory, "sorted-names-list.txt"))
                    .Should().Equal(["Marin Alvarez", "Janet Parsons"]);
            }
            finally
            {
                // Restore the process state even when the test fails before cleanup completes.
                Console.SetOut(originalOut);
                Console.SetError(originalError);
                Directory.SetCurrentDirectory(originalDirectory);
                Directory.Delete(temporaryDirectory, recursive: true);
            }
        }
    }

    // This test covers the CLI contract when the user does not provide required arguments.
    // The app should fail fast and tell the user how to invoke it correctly.
    [Fact]
    public void Main_MissingArguments_ReturnsInvalidArgumentsExitCode()
    {
        lock (TestProcessState.Lock)
        {
            var originalError = Console.Error;

            try
            {
                using var error = new StringWriter();
                Console.SetError(error);

                var exitCode = global::NameSorter.Program.Main([]);

                exitCode.Should().Be(1);
                error.ToString().Should().Contain("Usage: name-sorter <file-path>");
            }
            finally
            {
                Console.SetError(originalError);
            }
        }
    }

    // This test ensures missing input files are treated as a command-line error with a specific
    // exit code and a clear error message, rather than crashing unexpectedly.
    [Fact]
    public void Main_MissingFile_ReturnsInputFileExitCode()
    {
        lock (TestProcessState.Lock)
        {
            var originalError = Console.Error;

            try
            {
                using var error = new StringWriter();
                Console.SetError(error);

                var exitCode = global::NameSorter.Program.Main(["missing.txt"]);

                exitCode.Should().Be(2);
                error.ToString().Should().Contain("Error: File not found: missing.txt");
            }
            finally
            {
                Console.SetError(originalError);
            }
        }
    }
}