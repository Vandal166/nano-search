using Microsoft.Extensions.DependencyInjection;
using NanoSearch.Algorithms.Indexer;
using NanoSearch.Configuration.Indexing;

namespace NanoSearch.Configuration.Services;

public static class IndexingServiceCollectionExtensions
{
    public static IServiceCollection AddIndexing(this IServiceCollection services)
    {
        /*services.AddSingleton<IndexingOptions>();*/
        services.AddSingleton<IConfigService<IndexingOptions>>(
            _ => new JsonConfigService<IndexingOptions>("indexing_options.json"));

        services.AddSingleton<IConfigService<KeybindingsOptions>>(
            _ => new JsonConfigService<KeybindingsOptions>("keybindings.json"));
        
        services.AddSingleton<FilterPipeline>(sp =>
            FilterPipelineBuilder.Build(sp.GetRequiredService<IConfigService<IndexingOptions>>().Options)
        );
        
        services.AddSingleton<FileIndexer>(sp => 
        {
            var idx = new FileIndexer();
            var opts = sp.GetRequiredService<IConfigService<IndexingOptions>>().Options;
            idx.IndexFileSystem(opts.DrivesToIndex, sp.GetRequiredService<FilterPipeline>());
            return idx;
        });
        
        services.AddTransient<FileFilterOptions>(provider =>
            provider.GetRequiredService<IConfigService<IndexingOptions>>().Options.FileFilter);
        services.AddTransient<DirectoryFilterOptions>(provider =>
            provider.GetRequiredService<IConfigService<IndexingOptions>>().Options.DirectoryFilter);
        
        return services;
    }
}