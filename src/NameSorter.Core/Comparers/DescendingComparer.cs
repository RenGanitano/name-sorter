namespace NameSorter.Comparers;

/// <summary>
/// Reverses the ordering of an inner <see cref="IComparer{T}"/>.
/// </summary>
public class DescendingComparer<T> : IComparer<T>
{
    private readonly IComparer<T> inner;

    public DescendingComparer(IComparer<T> inner)
    {
        this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public int Compare(T? x, T? y) => this.inner.Compare(y, x);
}
