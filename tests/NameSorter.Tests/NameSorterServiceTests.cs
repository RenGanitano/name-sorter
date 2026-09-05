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
            new(new List<string> { "Alice" }, "Smith"),
            new(new List<string> { "Bob" }, "Jones"),
        };

        service.Sort(names);

        mockComparer.Verify(c => c.Compare(It.IsAny<PersonName>(), It.IsAny<PersonName>()), Times.AtLeastOnce);
    }

    [Fact]
    public void Sort_WithRealComparer_SortsCorrectly()
    {
        var service = new NameSorterService(new LastNameFirstComparer());
        var names = new List<PersonName>
        {
            new(new List<string> { "Janet" }, "Parsons"),
            new(new List<string> { "Marin" }, "Alvarez"),
            new(new List<string> { "Leo" }, "Gardner"),
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

        var sorted = service.Sort(new List<PersonName>());

        Assert.Empty(sorted);
    }

    [Fact]
    public void Sort_SingleName_ReturnsSingleName()
    {
        var service = new NameSorterService(new LastNameFirstComparer());
        var names = new List<PersonName>
        {
            new(new List<string> { "John" }, "Doe"),
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
            new(new List<string> { "Zoe" }, "Zander"),
            new(new List<string> { "Alice" }, "Adams"),
        };
        var originalFirst = names[0].ToString();

        service.Sort(names);

        Assert.Equal(originalFirst, names[0].ToString());
    }
}
