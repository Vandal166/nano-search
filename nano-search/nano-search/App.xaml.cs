using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using NanoSearch.Algorithms.Indexer;
using NanoSearch.Configuration;
using NanoSearch.Configuration.Indexing;
using Application = System.Windows.Application;

namespace NanoSearch;

public partial class App : Application
{
    private NotifyIcon? _trayIcon;
    private ServiceProvider _serviceProvider = null!;
    
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // DI setup
        var services = new ServiceCollection();
        services.AddServices();
        _serviceProvider = services.BuildServiceProvider();
        
        var searchWindow = _serviceProvider.GetRequiredService<SearchWindow>();
        _serviceProvider.GetRequiredService<SettingsWindow>();
        
        // if Show in Tray is true in the cfg then load the tray icon
        if (_serviceProvider.GetRequiredService<IndexingOptions>().ShowInTray)
        {
            _trayIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("Resources\\nano-search.ico"),
                Visible = true,
                Text = "nano-search"
            };
            
            _trayIcon.MouseClick += TrayIcon_MouseClick; // on click
            _trayIcon.DoubleClick += TrayIcon_DoubleClick; // on double click
        }

        searchWindow.Show();
    }

    private void TrayIcon_DoubleClick(object? sender, EventArgs e)
    {
        _serviceProvider.GetRequiredService<SearchWindow>().ShowAndFocus();
    }

    private void TrayIcon_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            var cursor = System.Windows.Forms.Control.MousePosition;
            var settingsWindow = _serviceProvider.GetRequiredService<SettingsWindow>();
            settingsWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            settingsWindow.Left = cursor.X - (settingsWindow.Width);
            settingsWindow.Top = cursor.Y - (settingsWindow.Height);
            settingsWindow.Show();
            settingsWindow.Activate();
        }
    }
    protected override void OnExit(ExitEventArgs e) 
    {
        _trayIcon?.Dispose();
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IndexingOptions>();
        services.AddSingleton<JsonConfigService>(provider =>
        {
            var options = provider.GetRequiredService<IndexingOptions>();
            var config = new JsonConfigService(options);
            config.Load();
            return config;
        });
        
        services.AddSingleton<FilterPipeline>(provider =>
        {
            var config = provider.GetRequiredService<JsonConfigService>();
            return FilterPipelineBuilder.Build(config.IndexingOptions);
        });
        services.AddSingleton<FileIndexer>(sp => {
          // initial indexing
          var idx = new FileIndexer();
          var indexingOptions = sp.GetRequiredService<IndexingOptions>();
          idx.IndexFileSystem(indexingOptions.DrivesToIndex, sp.GetRequiredService<FilterPipeline>());
          return idx;
      });

        services.AddSingleton<IAppLauncher, AppLauncher>();
        services.AddSingleton<AppLauncher>();
        services.AddSingleton<IIconLoader, IconLoader>();
        services.AddSingleton<ExplorerLauncher>();
        services.AddSingleton<LinkLauncher>();
        
        services.AddSingleton<IDialogService, DialogService>();
        services.AddTransient<FileFilterOptions>(provider =>
            provider.GetRequiredService<IndexingOptions>().FileFilter);
        services.AddTransient<FilterOptionsViewModel>();
        services.AddTransient<FilterOptionsWindow>();   // XAML window
        
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
        
        services.AddSingleton<SettingsWindow>(provider =>
        {
            var vm = provider.GetRequiredService<SettingsViewModel>();
            return new SettingsWindow(vm);
        });
        

        services.AddSingleton<ISearchService>(provider =>
        {
            var fileIndexer = provider.GetRequiredService<FileIndexer>();
            var launcher = provider.GetRequiredService<ExplorerLauncher>();
            var iconLoader = provider.GetRequiredService<IIconLoader>();
            return new SearchService(fileIndexer.RadixTree, launcher, iconLoader);
        });
        services.AddSingleton<ListBoxNavigateDownStrategy>();
        services.AddSingleton<ListboxNavigationUpStrategy>();
        services.AddSingleton<ListboxNavigationEnterStrategy>();
        services.AddSingleton<IListBoxNavigationStrategyFactory, ListBoxNavigationStrategyFactory>();
        
        services.AddSingleton<INavigationService>(provider =>
        {
            var factory = provider.GetRequiredService<IListBoxNavigationStrategyFactory>();
            var appLauncher = provider.GetRequiredService<IAppLauncher>();
            
            var strategies = factory.CreateStrategies(appLauncher);
            return new ListBoxNavigationService(strategies);
        });
        services.AddSingleton<SearchWindow>();
    }
}
