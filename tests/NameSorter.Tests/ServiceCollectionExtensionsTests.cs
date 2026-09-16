using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using NameSorter.Comparers;
using NameSorter.DependencyInjection;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddNameSorting_RegistersTheParseSortPipeline()
    {
        var services = new ServiceCollection();

        var returned = services.AddNameSorting();

        returned.Should().BeSameAs(services);

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<INameParser>().Should().BeOfType<NameParser>();
        provider.GetRequiredService<IComparer<PersonName>>().Should().BeOfType<LastNameFirstComparer>();
        provider.GetRequiredService<INameSorter>().Should().BeOfType<NameSorterService>();
        provider.GetRequiredService<INameSortingProcessor>().Should().BeOfType<NameSortingProcessor>();
    }
}
