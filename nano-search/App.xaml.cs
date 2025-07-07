using System.IO;
using System.Reflection;
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
    private static Mutex? _singleInstanceMutex;
    
    private NotifyIcon? _trayIcon;
    private ServiceProvider _serviceProvider = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        _singleInstanceMutex = new Mutex(true, "NanoSearchSingleInstance", out var isSingleInstance);
        if (!isSingleInstance)
        {
            Environment.Exit(0); // exiting if another instance is already running of the app
            return;
        }

        try
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
                var assembly = Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream("NanoSearch.Resources.nano-search.ico");
                _trayIcon = new NotifyIcon
                {
                    Icon = new System.Drawing.Icon(stream),
                    Visible = true,
                    Text = "nano-search"
                };

                _trayIcon.MouseClick += TrayIcon_MouseClick; // on click
                _trayIcon.DoubleClick += TrayIcon_DoubleClick; // on double click
            }

            searchWindow.Show();
            if (ApplicationThemeManager.GetSystemTheme() == SystemTheme.Light)
            {
                MessageBoxExtensions.Setup
                (
                    "Light Mode Detected",
                    "Light mode detected, switch your system to Dark mode as light mode is not supported yet."
                ).Display();
            }

            SystemThemeWatcher.Watch(searchWindow);
            _serviceProvider.GetRequiredService<IHotKeyService>();
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.log", ex.ToString());
            MessageBoxExtensions.Setup
            (
                "Error",
                "An error occurred while starting the application. Please check the error.log file for more details."
            ).Display();
        }
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
            
            if (!settingsWindow.IsVisible)
            {
                settingsWindow.Show();
                settingsWindow.Hide();
            }
            
            var source = PresentationSource.FromVisual(settingsWindow);
            if (source?.CompositionTarget != null)
            {
                var transform = source.CompositionTarget.TransformFromDevice;
                var point = transform.Transform(new System.Windows.Point(cursor.X, cursor.Y));
                settingsWindow.Left = point.X - settingsWindow.Width;
                settingsWindow.Top = point.Y - settingsWindow.Height;
            }
            else
            {
                // fallback
                settingsWindow.Left = cursor.X - settingsWindow.Width;
                settingsWindow.Top = cursor.Y - settingsWindow.Height;
            }
            
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