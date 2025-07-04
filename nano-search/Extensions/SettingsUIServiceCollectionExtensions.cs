using Microsoft.Extensions.DependencyInjection;
using NanoSearch.Algorithms.Indexer;
using NanoSearch.Configuration;
using NanoSearch.Configuration.Indexing;
using NanoSearch.Configuration.Keybindings;
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
        services.AddTransient<KeybindingsOptionsViewModel>();
        services.AddTransient<KeybindingsOptionsWindow>();
        
        services.AddTransient<FileFilterOptionsViewModel>();
        services.AddTransient<FileFilterOptionsWindow>();
        
        services.AddTransient<DirectoryFilterOptionsViewModel>();
        services.AddTransient<DirectoryFilterOptionsWindow>();

        // Settings panel
        services.AddSingleton<IndexingSettingsViewModel>(provider =>
        {
            var dialogService = provider.GetRequiredService<IDialogService>();
            var indexingConfigService = provider.GetRequiredService<IConfigService<IndexingOptions>>();
            var fileIndexer = provider.GetRequiredService<FileIndexer>();
            Action onOptionsChanged = () =>
            {
                var pipeline = FilterPipelineBuilder.Build(indexingConfigService.Options);
                provider.GetRequiredService<FileIndexer>().IndexFileSystem(indexingConfigService.Options.DrivesToIndex, pipeline);
            };
            return new IndexingSettingsViewModel(dialogService, indexingConfigService, fileIndexer, onOptionsChanged);
        });
        
        services.AddSingleton<KeybindingsSettingsViewModel>(provider =>
        {
            var dialogService = provider.GetRequiredService<IDialogService>();
            var keybindingsConfigService = provider.GetRequiredService<IConfigService<KeybindingsOptions>>();
            Action onOptionsChanged = () =>
            {
                //noop
            };
            return new KeybindingsSettingsViewModel(dialogService, keybindingsConfigService, onOptionsChanged);
        });
        
        services.AddSingleton<SettingsViewModel>(provider =>
        {
            var indexingSettings = provider.GetRequiredService<IndexingSettingsViewModel>();
            var keybindingsSettings = provider.GetRequiredService<KeybindingsSettingsViewModel>();
            return new SettingsViewModel(indexingSettings, keybindingsSettings);
        });
        services.AddSingleton<SettingsWindow>();

        return services;
    }
}