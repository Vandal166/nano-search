using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Forms;
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

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        _trayIcon = new NotifyIcon
        {
            Icon = new System.Drawing.Icon("Resources\\nano-search.ico"),
            Visible = true,
            Text = "nano-search"
        };
        
        _trayIcon.MouseClick += TrayIcon_MouseClick; // on click
        _trayIcon.DoubleClick += TrayIcon_DoubleClick; // on double click
        
        
        var indexingOptions = new IndexingOptions();
        
        var configService = new JsonConfigService("indexing_options.json", indexingOptions);
        configService.Load();
        var fileIndexer = new FileIndexer();
        
        FilterPipeline pipeline = FilterPipelineBuilder.Build(configService.IndexingOptions);
        //AnsiConsole.Write(new Rule("[yellow bold underline]Indexing[/]").LeftJustified().NewLine());
        /*AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Dots)
            .Start("[yellow]Indexing file system...[/]", ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.Status($"[bold blue]Starting file indexing on {IndexingOptions.DiskRootPath}[/]");
                
            });*/
        fileIndexer.IndexFileSystem(IndexingOptions.DiskRootPath, pipeline);
        
        _searchWindow = new SearchWindow(fileIndexer);
        _searchWindow.Show();
    }

    private void TrayIcon_DoubleClick(object? sender, EventArgs e)
    {
        _searchWindow.Show();
    }

    private void TrayIcon_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            // Show context menu
            // allow indexing
            // exit
        }
    }
    
    private void ShowSearchUI() 
    {
        /*var window = new SearchWindow(); // TODO cache to save resources
        window.Show();*/
    }

    protected override void OnExit(ExitEventArgs e) 
    {
        _trayIcon?.Dispose();
        base.OnExit(e);
    }
}