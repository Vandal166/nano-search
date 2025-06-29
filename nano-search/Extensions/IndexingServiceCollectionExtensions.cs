using Microsoft.Extensions.DependencyInjection;
using NanoSearch.Algorithms.Indexer;
using NanoSearch.Configuration.Indexing;

namespace NanoSearch.Configuration.Services;

public static class IndexingServiceCollectionExtensions
{
    public static IServiceCollection AddIndexing(this IServiceCollection services)
    {
        services.AddSingleton<IndexingOptions>();
        services.AddSingleton<JsonConfigService>(provider =>
        {
            var options = provider.GetRequiredService<IndexingOptions>();
            var config = new JsonConfigService(options);
            config.Load();
            return config;
        });
        
        services.AddSingleton<FilterPipeline>(sp =>
            FilterPipelineBuilder.Build(sp.GetRequiredService<JsonConfigService>().IndexingOptions)
        );
        
        services.AddSingleton<FileIndexer>(sp => 
        {
            var idx = new FileIndexer();
            var opts = sp.GetRequiredService<IndexingOptions>();
            idx.IndexFileSystem(opts.DrivesToIndex, sp.GetRequiredService<FilterPipeline>());
            return idx;
        });
        
        services.AddTransient<FileFilterOptions>(provider =>
            provider.GetRequiredService<IndexingOptions>().FileFilter);
        services.AddTransient<DirectoryFilterOptions>(provider =>
            provider.GetRequiredService<IndexingOptions>().DirectoryFilter);
        
        return services;
    }
}