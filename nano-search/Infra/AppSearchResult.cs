using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using NanoSearch.Launchers;

namespace NanoSearch.Services;

public class AppSearchResult : INotifyPropertyChanged
{
    private readonly IAppLauncher _explorerLauncher;
    private readonly IIconLoader _iconLoader;
    
    public string FileName { get; set; }
    public string FullPath { get; }
    private ImageSource? _icon;
    public ImageSource? Icon
    {
        get => _icon;
        set
        {
            _icon = value; 
            OnPropertyChanged();
        }
    }
    
    public ICommand OpenCommand { get; }
    
    public AppSearchResult(string fullPath, IAppLauncher explorerLauncher, IIconLoader iconLoader)
    {
        FullPath = fullPath;
        FileName = Path.GetFileName(fullPath);
        _explorerLauncher = explorerLauncher;
        _iconLoader = iconLoader;
        OpenCommand = new RelayCommand(OpenInExplorer);
        _ = LoadIconAsync();
    }

    private void OpenInExplorer()
    {
        _explorerLauncher.Launch(FullPath);
    }

    private async Task LoadIconAsync()
    {
       Icon = await _iconLoader.LoadIconAsync(FullPath);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? property = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
}