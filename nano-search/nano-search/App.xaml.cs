using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace NanoSearch;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private NotifyIcon _trayIcon;


    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        _trayIcon = new NotifyIcon
        {
            Icon = new System.Drawing.Icon("resources\\nano-search.ico"),
            Visible = true,
            Text = "nano-search"
        };
        
        _trayIcon.MouseClick += TrayIcon_MouseClick; // on click
        _trayIcon.DoubleClick += TrayIcon_DoubleClick; // on double click
    }

    private void TrayIcon_DoubleClick(object? sender, EventArgs e)
    {
        //show search window
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
        var window = new SearchWindow(); // TODO cache to save resources
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e) 
    {
        _trayIcon.Dispose();
        base.OnExit(e);
    }
}