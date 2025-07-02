using Microsoft.Extensions.DependencyInjection;
using NanoSearch.Algorithms.Indexer;
using NanoSearch.Configuration.Indexing;
using NanoSearch.Configuration.Keybindings;
using NanoSearch.Configuration.Validators;

namespace NanoSearch.Configuration.Services;

public static class IndexingServiceCollectionExtensions
{
    public static IServiceCollection AddIndexing(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<KeybindingsOptions>, KeybindingsOptionsValidator>();
        services.AddSingleton<IValidator<FileFilterOptions>, FileFilterOptionsValidator>();
        services.AddSingleton<IValidator<DirectoryFilterOptions>, DirectoryFilterOptionsValidator>();
        services.AddSingleton<IValidator<IndexingOptions>, IndexingOptionsValidator>();
//etc
        services.AddSingleton<IConfigService<IndexingOptions>>(p =>
            new JsonConfigService<IndexingOptions>("indexing_options.json", p.GetRequiredService<IValidator<IndexingOptions>>())
        );

        services.AddSingleton<IConfigService<KeybindingsOptions>>(p =>
            new JsonConfigService<KeybindingsOptions>("keybindings.json", p.GetRequiredService<IValidator<KeybindingsOptions>>())
        );
        
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