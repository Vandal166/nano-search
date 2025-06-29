using Microsoft.Extensions.DependencyInjection;
using NanoSearch.Algorithms.Indexer;
using NanoSearch.Configuration;
using NanoSearch.Configuration.Indexing;
using NanoSearch.UI.Services;
using NanoSearch.UI.ViewModels;
using NanoSearch.UI.Windows;

namespace NanoSearch.Configuration.Services;

public static class SettingsUIServiceCollectionExtensions
{
    public static IServiceCollection AddSettingsUI(this IServiceCollection services)
    {
        services.AddSingleton<IDialogService, DialogService>();

        // Filter options dialogs
        services.AddTransient<FileFilterOptionsViewModel>();
        services.AddTransient<FileFilterOptionsWindow>();
        services.AddTransient<DirectoryFilterOptionsViewModel>();
        services.AddTransient<DirectoryFilterOptionsWindow>();

        // Settings panel
        services.AddSingleton<SettingsViewModel>(provider =>
        {
            var dialogService = provider.GetRequiredService<IDialogService>();
            var options = provider.GetRequiredService<IndexingOptions>();
            var configService = provider.GetRequiredService<JsonConfigService>();
            var fileIndexer = provider.GetRequiredService<FileIndexer>();
            Action onOptionsChanged = () =>
            {
                var pipeline = FilterPipelineBuilder.Build(configService.IndexingOptions);
                fileIndexer.IndexFileSystem(options.DrivesToIndex, pipeline);
            };
            return new SettingsViewModel(dialogService, options, configService, fileIndexer, onOptionsChanged);
        });
        services.AddSingleton<SettingsWindow>();

        return services;
    }
}