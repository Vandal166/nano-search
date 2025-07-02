using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using NanoSearch.Configuration;
using NanoSearch.Configuration.Indexing;
using NanoSearch.Configuration.Keybindings;
using NanoSearch.Configuration.Services;
using NanoSearch.Navigation.Hotkey;
using NanoSearch.UI.Windows;
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
        var services = new ServiceCollection()
            .AddIndexing()
            .AddLaunchers()
            .AddSearchUI()
            .AddSettingsUI()
            .AddNavigation();
        
        _serviceProvider = services.BuildServiceProvider();
        var searchWindow = _serviceProvider.GetRequiredService<SearchWindow>();
        
        // if Show in Tray is true in the cfg then load the tray icon
        if (_serviceProvider.GetRequiredService<IConfigService<IndexingOptions>>().Options.ShowInTray)
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
        _serviceProvider.GetRequiredService<IHotKeyService>();
        _serviceProvider.GetRequiredService<ValidationErrorBox<KeybindingsOptions>>();
        _serviceProvider.GetRequiredService<ValidationErrorBox<IndexingOptions>>();
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