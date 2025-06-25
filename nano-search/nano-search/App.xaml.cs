using System.Windows;
using NanoSearch.Algorithms.Indexer;
using NanoSearch.Configuration;
using NanoSearch.Configuration.Indexing;
using Application = System.Windows.Application;

namespace NanoSearch;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private NotifyIcon? _trayIcon;
    private SearchWindow _searchWindow { get; set; }
    private SettingsWindow _settingsWindow;
    
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        var indexingOptions = new IndexingOptions();
        var configService = new JsonConfigService("indexing_options.json", indexingOptions);
        configService.Load();
        
        var fileIndexer = new FileIndexer();
        FilterPipeline pipeline = FilterPipelineBuilder.Build(configService.IndexingOptions);
        fileIndexer.IndexFileSystem(IndexingOptions.DiskRootPath, pipeline);
        
        
        var _appLauncher = new AppLauncher();
        var _navigation = new ListBoxNavigationService(_appLauncher);
        
        _searchWindow = new SearchWindow(
            new SearchService(fileIndexer.RadixTree, 
                new ExplorerLauncher(), 
                new IconLoader()), 
            _appLauncher, _navigation);
        _searchWindow.Show();

        _settingsWindow = new SettingsWindow(indexingOptions, configService);
        
        _trayIcon = new NotifyIcon
        {
            Icon = new System.Drawing.Icon("Resources\\nano-search.ico"),
            Visible = true,
            Text = "nano-search"
        };
        
        _trayIcon.MouseClick += TrayIcon_MouseClick; // on click
        _trayIcon.DoubleClick += TrayIcon_DoubleClick; // on double click

    }

    private void TrayIcon_DoubleClick(object? sender, EventArgs e)
    {
        _searchWindow.ShowAndFocus();
    }

    private void TrayIcon_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            var cursor = System.Windows.Forms.Control.MousePosition;
            _settingsWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            _settingsWindow.Left = cursor.X - (_settingsWindow.Width / 2);
            _settingsWindow.Top = cursor.Y - (_settingsWindow.Height);
            _settingsWindow.Show();
            _settingsWindow.Activate();
        }
    }
    protected override void OnExit(ExitEventArgs e) 
    {
        _trayIcon?.Dispose();
        base.OnExit(e);
    }
}