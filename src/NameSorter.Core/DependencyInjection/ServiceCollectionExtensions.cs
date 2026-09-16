using Microsoft.Extensions.DependencyInjection;
using NameSorter.Comparers;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the parse → sort pipeline. Hosts add their own <see cref="INameReader"/>
    /// and <see cref="INameWriter"/> implementations on top.
    /// </summary>
    public static IServiceCollection AddNameSorting(this IServiceCollection services)
    {
        services.AddSingleton<INameParser, NameParser>();
        services.AddSingleton<IComparer<PersonName>, LastNameFirstComparer>();
        services.AddSingleton<INameSorter, NameSorterService>();
        services.AddSingleton<INameSortingProcessor, NameSortingProcessor>();
        return services;
    }
}
