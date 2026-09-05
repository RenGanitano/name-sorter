using AwesomeAssertions;
using Moq;
using NameSorter.Comparers;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter.Tests;

public class NameSorterServiceTests
{
    [Fact]
    public void Constructor_NullComparer_ThrowsArgumentNullException()
    {
        Action act = () => new NameSorterService(null!);

        act.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("comparer");
    }

    [Fact]
    public void Sort_DelegatesToInjectedComparer()
    {
        var mockComparer = new Mock<IComparer<PersonName>>();
        mockComparer
            .Setup(c => c.Compare(It.IsAny<PersonName>(), It.IsAny<PersonName>()))
            .Returns(0);

        var service = new NameSorterService(mockComparer.Object);
        var names = new List<PersonName>
        {
            new(["Alice"], "Smith"),
            new(["Bob"], "Jones"),
        };

        service.Sort(names);

        mockComparer.Verify(c => c.Compare(It.IsAny<PersonName>(), It.IsAny<PersonName>()), Times.AtLeastOnce);
    }

    [Fact]
    public void Sort_UnorderedNames_ReturnsNamesOrderedByLastName()
    {
        var service = new NameSorterService(new LastNameFirstComparer());
        var names = new List<PersonName>
        {
            new(["Janet"], "Parsons"),
            new(["Marin"], "Alvarez"),
            new(["Leo"], "Gardner"),
        };

        var sorted = service.Sort(names);

        sorted[0].ToString().Should().Be("Marin Alvarez");
        sorted[1].ToString().Should().Be("Leo Gardner");
        sorted[2].ToString().Should().Be("Janet Parsons");
    }

    [Fact]
    public void Sort_EmptyList_ReturnsEmptyList()
    {
        var service = new NameSorterService(new LastNameFirstComparer());

        var sorted = service.Sort([]);

        sorted.Should().BeEmpty();
    }

    [Fact]
    public void Sort_SingleName_ReturnsSingleName()
    {
        var service = new NameSorterService(new LastNameFirstComparer());
        var names = new List<PersonName>
        {
            new(["John"], "Doe"),
        };

        var sorted = service.Sort(names);

        sorted.Should().ContainSingle();
        sorted[0].ToString().Should().Be("John Doe");
    }

    [Fact]
    public void Sort_DoesNotMutateOriginalList()
    {
        var service = new NameSorterService(new LastNameFirstComparer());
        var names = new List<PersonName>
        {
            new(["Zoe"], "Zander"),
            new(["Alice"], "Adams"),
        };
        var originalNames = names.Select(name => name.ToString()).ToList();

        service.Sort(names);

        names.Select(name => name.ToString()).Should().Equal(originalNames);
    }
}
