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
        services.AddSingleton<SettingsViewModel>(provider =>
        {
            var dialogService = provider.GetRequiredService<IDialogService>();
            var indexingConfigService = provider.GetRequiredService<IConfigService<IndexingOptions>>();
            var keybindingsConfigService = provider.GetRequiredService<IConfigService<KeybindingsOptions>>();
            var fileIndexer = provider.GetRequiredService<FileIndexer>();
            Action onOptionsChanged = () =>
            {
                var pipeline = FilterPipelineBuilder.Build(indexingConfigService.Options);
                fileIndexer.IndexFileSystem(indexingConfigService.Options.DrivesToIndex, pipeline);
                keybindingsConfigService.Load();
            };
            return new SettingsViewModel(dialogService, indexingConfigService, keybindingsConfigService, fileIndexer, onOptionsChanged);
        });
        services.AddSingleton<SettingsWindow>();
        
        services.AddSingleton<ValidationErrorBox<KeybindingsOptions>>(provider =>
        {
            var configService = provider.GetRequiredService<IConfigService<KeybindingsOptions>>();
            return new ValidationErrorBox<KeybindingsOptions>(configService);
        });
        services.AddSingleton<ValidationErrorBox<IndexingOptions>>(provider =>
        {
            var configService = provider.GetRequiredService<IConfigService<IndexingOptions>>();
            return new ValidationErrorBox<IndexingOptions>(configService);
        });

        return services;
    }
}