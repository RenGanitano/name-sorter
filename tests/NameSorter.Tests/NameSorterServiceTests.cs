using Moq;
using NameSorter.Comparers;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter.Tests;

public class NameSorterServiceTests
{
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

        Assert.Equal("Marin Alvarez", sorted[0].ToString());
        Assert.Equal("Leo Gardner", sorted[1].ToString());
        Assert.Equal("Janet Parsons", sorted[2].ToString());
    }

    [Fact]
    public void Sort_EmptyList_ReturnsEmptyList()
    {
        var service = new NameSorterService(new LastNameFirstComparer());

        var sorted = service.Sort([]);

        Assert.Empty(sorted);
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

        Assert.Single(sorted);
        Assert.Equal("John Doe", sorted[0].ToString());
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

        Assert.Equal(originalNames, names.Select(name => name.ToString()).ToList());
    }

    [Fact]
    public void Sort_EqualNames_PreservesInputOrder()
    {
        var service = new NameSorterService(new LastNameFirstComparer());
        var first = new PersonName(["Alex"], "Smith");
        var second = new PersonName(["alex"], "SMITH");

        var sorted = service.Sort([first, second]);

        Assert.Same(first, sorted[0]);
        Assert.Same(second, sorted[1]);
    }
}
