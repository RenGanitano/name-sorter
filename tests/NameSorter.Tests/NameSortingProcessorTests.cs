using AwesomeAssertions;
using Moq;
using NameSorter.Comparers;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter.Tests;

public class NameSortingProcessorTests
{
    [Fact]
    public void Constructor_NullParser_ThrowsArgumentNullException()
    {
        Action act = () => new NameSortingProcessor(null!, Mock.Of<INameSorter>());

        act.Should().Throw<ArgumentNullException>().WithParameterName("parser");
    }

    [Fact]
    public void Constructor_NullSorter_ThrowsArgumentNullException()
    {
        Action act = () => new NameSortingProcessor(Mock.Of<INameParser>(), null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("sorter");
    }

    [Fact]
    public void Process_ValidInput_ParsesEachNameAndReturnsSortedResult()
    {
        // Arrange
        var parsedNames = new List<PersonName>
        {
            new(["Janet"], "Parsons"),
            new(["Marin"], "Alvarez"),
        };
        var mockParser = new Mock<INameParser>();
        mockParser.Setup(p => p.Parse("Janet Parsons")).Returns(new NameParseResult(parsedNames[0], null));
        mockParser.Setup(p => p.Parse("Marin Alvarez")).Returns(new NameParseResult(parsedNames[1], null));

        var sortedNames = new List<PersonName> { parsedNames[1], parsedNames[0] };
        var mockSorter = new Mock<INameSorter>();
        mockSorter
            .Setup(s => s.Sort(It.IsAny<IList<PersonName>>(), SortDirection.Ascending))
            .Returns(sortedNames);

        var processor = new NameSortingProcessor(mockParser.Object, mockSorter.Object);

        // Act
        var result = processor.Process(["Janet Parsons", "Marin Alvarez"]);

        // Assert
        mockParser.Verify(p => p.Parse("Janet Parsons"), Times.Once);
        mockParser.Verify(p => p.Parse("Marin Alvarez"), Times.Once);
        mockSorter.Verify(s => s.Sort(
            It.Is<IList<PersonName>>(list => list.SequenceEqual(parsedNames)),
            SortDirection.Ascending), Times.Once);
        result.SortedNames.Should().BeSameAs(sortedNames);
        result.InvalidNames.Should().BeEmpty();
    }

    [Fact]
    public void Process_InvalidNames_ExcludesThemBeforeSortingAndReportsTruePosition()
    {
        // Arrange: an ignored blank entry sits between the valid and invalid names, so the
        // invalid name is at position 3 even though it is the second parseable entry.
        var parsedName = new PersonName(["Janet"], "Parsons");
        var mockParser = new Mock<INameParser>();
        mockParser.Setup(p => p.Parse("Janet Parsons")).Returns(new NameParseResult(parsedName, null));
        mockParser.Setup(p => p.Parse("")).Returns(new NameParseResult(null, null));
        mockParser.Setup(p => p.Parse("InvalidSingleName")).Returns(new NameParseResult(null, "invalid"));

        var mockSorter = new Mock<INameSorter>();
        mockSorter
            .Setup(s => s.Sort(It.IsAny<IList<PersonName>>(), It.IsAny<SortDirection>()))
            .Returns<IList<PersonName>, SortDirection>((names, _) => names);

        var processor = new NameSortingProcessor(mockParser.Object, mockSorter.Object);

        // Act
        var result = processor.Process(["Janet Parsons", "", "InvalidSingleName"]);

        // Assert
        mockSorter.Verify(s => s.Sort(
            It.Is<IList<PersonName>>(list => list.Count == 1 && list[0] == parsedName),
            SortDirection.Ascending), Times.Once);

        result.SortedNames.Should().ContainSingle().Which.Should().BeSameAs(parsedName);
        result.InvalidNames.Should().ContainSingle()
            .Which.Should().Be(new InvalidName(3, "InvalidSingleName", "invalid"));
    }

    [Fact]
    public void Process_Descending_PassesDirectionToSorter()
    {
        var mockParser = new Mock<INameParser>();
        mockParser.Setup(p => p.Parse(It.IsAny<string>())).Returns(new NameParseResult(null, null));

        var mockSorter = new Mock<INameSorter>();
        mockSorter
            .Setup(s => s.Sort(It.IsAny<IList<PersonName>>(), SortDirection.Descending))
            .Returns([]);

        var processor = new NameSortingProcessor(mockParser.Object, mockSorter.Object);

        processor.Process([""], SortDirection.Descending);

        mockSorter.Verify(s => s.Sort(It.IsAny<IList<PersonName>>(), SortDirection.Descending), Times.Once);
    }

    [Fact]
    public void Process_EmptyInput_ReturnsEmptyResult()
    {
        var processor = new NameSortingProcessor(new NameParser(), new NameSorterService(new LastNameFirstComparer()));

        var result = processor.Process([]);

        result.SortedNames.Should().BeEmpty();
        result.InvalidNames.Should().BeEmpty();
    }

    // End-to-end through the real parser, comparer and sorter: the same pipeline the CLI
    // and API share, without any I/O.
    [Fact]
    public void Process_RealPipeline_SortsByLastNameThenGivenNames()
    {
        var processor = new NameSortingProcessor(new NameParser(), new NameSorterService(new LastNameFirstComparer()));

        var result = processor.Process(["Janet Parsons", "Adonis Julius Archer", "Marin Alvarez", "Bad"]);

        result.SortedNames.Select(n => n.ToString())
            .Should().Equal("Marin Alvarez", "Adonis Julius Archer", "Janet Parsons");
        result.InvalidNames.Should().ContainSingle()
            .Which.Should().Be(new InvalidName(4, "Bad", "expected 2-4 parts, got 1"));
    }
}
