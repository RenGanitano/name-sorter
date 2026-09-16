using AwesomeAssertions;
using Moq;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter.Tests;

public class NameSortingApplicationTests
{
    private static readonly List<string> RawNames = ["Janet Parsons", "Marin Alvarez"];

    private static readonly NameSortingResult ProcessedResult = new(
        [new PersonName(["Marin"], "Alvarez"), new PersonName(["Janet"], "Parsons")],
        [new InvalidName(3, "InvalidSingleName", "invalid")]);

    [Fact]
    public void Run_ReadsFileProcessesNamesAndWritesResultToAllWriters()
    {
        // Arrange
        var mockReader = new Mock<INameReader>();
        mockReader.Setup(r => r.Read("test.txt")).Returns(RawNames);

        var mockProcessor = new Mock<INameSortingProcessor>();
        mockProcessor.Setup(p => p.Process(RawNames, SortDirection.Ascending)).Returns(ProcessedResult);

        var mockWriter1 = new Mock<INameWriter>();
        var mockWriter2 = new Mock<INameWriter>();

        var app = new NameSortingApplication(
            mockReader.Object,
            mockProcessor.Object,
            new[] { mockWriter1.Object, mockWriter2.Object });

        // Act
        var result = app.Run("test.txt");

        // Assert: the processor's result is written to every writer and returned unchanged,
        // so the caller can still report the invalid lines.
        mockReader.Verify(r => r.Read("test.txt"), Times.Once);
        mockProcessor.Verify(p => p.Process(RawNames, SortDirection.Ascending), Times.Once);
        mockWriter1.Verify(w => w.Write(ProcessedResult.SortedNames), Times.Once);
        mockWriter2.Verify(w => w.Write(ProcessedResult.SortedNames), Times.Once);
        result.Should().BeSameAs(ProcessedResult);
    }

    [Fact]
    public void Run_Descending_PassesDirectionToProcessor()
    {
        var mockReader = new Mock<INameReader>();
        mockReader.Setup(r => r.Read("test.txt")).Returns(RawNames);

        var mockProcessor = new Mock<INameSortingProcessor>();
        mockProcessor.Setup(p => p.Process(RawNames, SortDirection.Descending)).Returns(ProcessedResult);

        var app = new NameSortingApplication(mockReader.Object, mockProcessor.Object, []);

        app.Run("test.txt", SortDirection.Descending);

        mockProcessor.Verify(p => p.Process(RawNames, SortDirection.Descending), Times.Once);
    }
}
