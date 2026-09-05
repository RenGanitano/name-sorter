using Moq;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter.Tests;

public class NameSortingApplicationTests
{
    [Fact]
    public void Run_OrchestratesReadParseSortWrite()
    {
        // Arrange
        var rawNames = new List<string> { "Janet Parsons", "Marin Alvarez" };

        var mockReader = new Mock<INameReader>();
        mockReader.Setup(r => r.Read("test.txt")).Returns(rawNames);

        var parsedNames = new List<PersonName>
        {
            new(new List<string> { "Janet" }, "Parsons"),
            new(new List<string> { "Marin" }, "Alvarez"),
        };
        var mockParser = new Mock<INameParser>();
        mockParser.Setup(p => p.Parse("Janet Parsons")).Returns(parsedNames[0]);
        mockParser.Setup(p => p.Parse("Marin Alvarez")).Returns(parsedNames[1]);

        var sortedNames = new List<PersonName> { parsedNames[1], parsedNames[0] };
        var mockSorter = new Mock<INameSorter>();
        mockSorter.Setup(s => s.Sort(It.IsAny<IList<PersonName>>())).Returns(sortedNames);

        var mockWriter1 = new Mock<INameWriter>();
        var mockWriter2 = new Mock<INameWriter>();

        var app = new NameSortingApplication(
            mockReader.Object,
            mockParser.Object,
            mockSorter.Object,
            new[] { mockWriter1.Object, mockWriter2.Object });

        // Act
        app.Run("test.txt");

        // Assert
        mockReader.Verify(r => r.Read("test.txt"), Times.Once);
        mockParser.Verify(p => p.Parse("Janet Parsons"), Times.Once);
        mockParser.Verify(p => p.Parse("Marin Alvarez"), Times.Once);
        mockSorter.Verify(s => s.Sort(It.IsAny<IList<PersonName>>()), Times.Once);
        mockWriter1.Verify(w => w.Write(sortedNames), Times.Once);
        mockWriter2.Verify(w => w.Write(sortedNames), Times.Once);
    }

    [Fact]
    public void Run_SkipsInvalidNames()
    {
        var rawNames = new List<string> { "Janet Parsons", "InvalidSingleName" };

        var mockReader = new Mock<INameReader>();
        mockReader.Setup(r => r.Read("test.txt")).Returns(rawNames);

        var parsedName = new PersonName(new List<string> { "Janet" }, "Parsons");
        var mockParser = new Mock<INameParser>();
        mockParser.Setup(p => p.Parse("Janet Parsons")).Returns(parsedName);
        mockParser.Setup(p => p.Parse("InvalidSingleName")).Returns((PersonName?)null);

        var mockSorter = new Mock<INameSorter>();
        mockSorter
            .Setup(s => s.Sort(It.IsAny<IList<PersonName>>()))
            .Returns<IList<PersonName>>(names => names);

        var mockWriter = new Mock<INameWriter>();

        var app = new NameSortingApplication(
            mockReader.Object,
            mockParser.Object,
            mockSorter.Object,
            new[] { mockWriter.Object });

        app.Run("test.txt");

        // Verify sorter received only 1 valid name
        mockSorter.Verify(s => s.Sort(It.Is<IList<PersonName>>(
            list => list.Count == 1 && list[0] == parsedName)), Times.Once);
    }
}
