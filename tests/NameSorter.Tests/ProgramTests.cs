namespace NameSorter.Tests;

public class ProgramTests
{
    private static readonly object ConsoleAndFileLock = new();

    [Fact]
    public void Main_ValidFile_SortsWritesAndReportsInvalidLines()
    {
        lock (ConsoleAndFileLock)
        {
            var originalDirectory = Directory.GetCurrentDirectory();
            var originalOut = Console.Out;
            var originalError = Console.Error;
            var temporaryDirectory = Path.Combine(Path.GetTempPath(), $"name-sorter-{Guid.NewGuid():N}");
            Directory.CreateDirectory(temporaryDirectory);

            try
            {
                var inputPath = Path.Combine(temporaryDirectory, "input.txt");
                File.WriteAllLines(inputPath, ["Janet Parsons", "InvalidSingleName", "Marin Alvarez"]);
                Directory.SetCurrentDirectory(temporaryDirectory);

                using var output = new StringWriter();
                using var error = new StringWriter();
                Console.SetOut(output);
                Console.SetError(error);

                var exitCode = global::NameSorter.Program.Main([inputPath]);

                Assert.Equal(0, exitCode);
                Assert.Equal("Marin Alvarez\nJanet Parsons\n", output.ToString());
                Assert.Contains("Warning: Line 2", error.ToString());
                Assert.Contains("Processed 2 valid name(s); skipped 1 invalid line(s).", error.ToString());
                Assert.Equal(
                    ["Marin Alvarez", "Janet Parsons"],
                    File.ReadAllLines(Path.Combine(temporaryDirectory, "sorted-names-list.txt")));
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetError(originalError);
                Directory.SetCurrentDirectory(originalDirectory);
                Directory.Delete(temporaryDirectory, recursive: true);
            }
        }
    }

    [Fact]
    public void Main_MissingArguments_ReturnsInvalidArgumentsExitCode()
    {
        lock (ConsoleAndFileLock)
        {
            var originalError = Console.Error;

            try
            {
                using var error = new StringWriter();
                Console.SetError(error);

                var exitCode = global::NameSorter.Program.Main([]);

                Assert.Equal(1, exitCode);
                Assert.Contains("Usage: name-sorter <file-path>", error.ToString());
            }
            finally
            {
                Console.SetError(originalError);
            }
        }
    }

    [Fact]
    public void Main_MissingFile_ReturnsInputFileExitCode()
    {
        lock (ConsoleAndFileLock)
        {
            var originalError = Console.Error;

            try
            {
                using var error = new StringWriter();
                Console.SetError(error);

                var exitCode = global::NameSorter.Program.Main(["missing.txt"]);

                Assert.Equal(2, exitCode);
                Assert.Contains("Error: File not found: missing.txt", error.ToString());
            }
            finally
            {
                Console.SetError(originalError);
            }
        }
    }
}