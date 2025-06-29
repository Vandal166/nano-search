using Microsoft.Extensions.DependencyInjection;
using NanoSearch.Algorithms.Indexer;
using NanoSearch.Launchers;
using NanoSearch.Services;
using NanoSearch.UI.Windows;

namespace NanoSearch.Configuration.Services;

public static class SearchUIServiceCollectionExtensions
{
    public static IServiceCollection AddSearchUI(this IServiceCollection services)
    {
        services.AddSingleton<ISearchService>(provider =>
        {
            var fileIndexer = provider.GetRequiredService<FileIndexer>();
            var launcher = provider.GetRequiredService<ExplorerLauncher>();
            var iconLoader = provider.GetRequiredService<IIconLoader>();
            return new SearchService(fileIndexer.RadixTree, launcher, iconLoader);
        });
        services.AddSingleton<SearchWindow>(); // register the interface
        return services;
    }
}