using AwesomeAssertions;
using NameSorter.Comparers;

namespace NameSorter.Tests;

public class DescendingComparerTests
{
    private readonly DescendingComparer<int> comparer = new(Comparer<int>.Default);

    [Fact]
    public void Compare_ReversesInnerOrdering()
    {
        comparer.Compare(1, 2).Should().BePositive();
        comparer.Compare(2, 1).Should().BeNegative();
    }

    [Fact]
    public void Compare_EqualValues_ReturnsZero()
    {
        comparer.Compare(3, 3).Should().Be(0);
    }

    [Fact]
    public void Constructor_NullInner_ThrowsArgumentNullException()
    {
        Action act = () => new DescendingComparer<int>(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("inner");
    }
}
