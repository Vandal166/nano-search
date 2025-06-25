using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using NanoSearch.Configuration;
using NanoSearch.Configuration.Indexing;
using Wpf.Ui.Controls;
using Application = System.Windows.Application;

namespace NanoSearch;

// SettingsWindow.xaml.cs
public partial class SettingsWindow : Window
{
    private readonly IndexingOptions _options;
    private readonly JsonConfigService _configService;

    public SettingsWindow(IndexingOptions opts, JsonConfigService cfg)
    {
        InitializeComponent();
        _options = opts;
        _configService = cfg;
        DataContext = _options;  // bind directly to your POCO(

        // e.g. if you need a Drives collection on the options:
        // _options.Drives = DriveInfo.GetDrives()
        //    .Select(d => new DriveOption(d.Name)).ToList();
    }

    private void OnLink(object s, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri){UseShellExecute=true});
        e.Handled = true;
    }

    private void OnOpenFileOptions(object s, RoutedEventArgs e)
    {
    }

    private void OnOpenDirOptions(object s, RoutedEventArgs e)
    {
        
    }

    private void OnExit(object s, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        // persist any changes
        _configService.Save();
        base.OnClosing(e);
    }
}
