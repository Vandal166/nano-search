using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using NanoSearch.Configuration;
using NanoSearch.Configuration.Indexing;
using NanoSearch.Configuration.Keybindings;
using NanoSearch.Configuration.Services;
using NanoSearch.Navigation.Hotkey;
using NanoSearch.UI;
using NanoSearch.UI.Windows;
using Wpf.Ui.Appearance;
using Application = System.Windows.Application;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace NanoSearch;

public partial class App : Application
{
    private NotifyIcon? _trayIcon;
    private ServiceProvider _serviceProvider = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ApplicationThemeManager.ApplySystemTheme(updateAccent: true);
           
        // DI setup
        var services = new ServiceCollection()
            .AddIndexing()
            .AddLaunchers()
            .AddSearchUI()
            .AddSettingsUI()
            .AddNavigation();
        
        _serviceProvider = services.BuildServiceProvider();
        _serviceProvider.GetRequiredService<ValidationErrorBox<IndexingOptions>>();
        _serviceProvider.GetRequiredService<ValidationErrorBox<KeybindingsOptions>>();
        
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
        if (ApplicationThemeManager.GetSystemTheme() == SystemTheme.Light)
        {
            var messageBox = new MessageBox
            {
                Title = "Light Mode Detected",
                Content = "Light mode detected, switch your system to Dark mode as light mode is not supported yet.",
                CloseButtonText = "OK",
                ShowTitle = true
            };
            messageBox.ShowDialogAsync();
        }
        SystemThemeWatcher.Watch(searchWindow);
        _serviceProvider.GetRequiredService<IHotKeyService>();
        
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