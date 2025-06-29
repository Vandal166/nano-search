using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using NanoSearch.Configuration;
using NanoSearch.Configuration.Indexing;
using NanoSearch.Services;
using NanoSearch.UI.Services;
using NanoSearch.UI.ViewModels.Properties;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Application = System.Windows.Application;

// ReSharper disable MemberCanBePrivate.Global
namespace NanoSearch.UI.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    public ObservableCollection<DriveOption> AllDrives { get; }
    public IndexedFileCount IndexedFileCount { get; }
    public ShowInTrayOption ShowInTrayOption { get; }
    
    public ISnackbarService snackbarService { get; }
    public IContentDialogService snackDialogService { get; }
    
    public ICommand TrayToggledCommand        { get; }
    public ICommand DriveCheckedCommand       { get; }
    public ICommand IndexFilesCommand         { get; }
    public ICommand OpenFileOptionsCommand    { get; }
    public ICommand OpenDirOptionsCommand     { get; }
    public ICommand ExitCommand               { get; }
    
    private readonly IDialogService _dialogService;
    private readonly IndexingOptions _options;
    private readonly JsonConfigService _configService;
    private readonly IFileCountProvider _fileCountProvider;
    private readonly Action _onOptionsChanged;
    private bool _isIndexing;
    public bool IsIndexing
    {
        get => _isIndexing;
        set
        {
            _isIndexing = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsIndexing)));
        }
    }

    private async Task OnOptionsChanged()
    {
        IsIndexing = true;
        try
        {
            await Task.Run(() => _onOptionsChanged());
        }
        finally
        {
            IsIndexing = false;
            IndexedFileCount.Count = _fileCountProvider.Count;
        }
    }
    public SettingsViewModel(IDialogService dialogService, IndexingOptions options, JsonConfigService configService, IFileCountProvider  fileCountProvider, Action onOptionsChanged)
    {
        _dialogService          = dialogService;
        _options            = options;
        _configService             = configService;
        _fileCountProvider  = fileCountProvider;
        _onOptionsChanged   = onOptionsChanged;

        snackDialogService = new ContentDialogService();
        snackbarService = new SnackbarService();
        
        // Populate AllDrives
        AllDrives = new ObservableCollection<DriveOption>(
            DriveInfo.GetDrives()
                .Where(d => d.DriveType is DriveType.Fixed or DriveType.Removable)
                .Select(d => d.Name) // e.g. "C:\"
                .Select(letter => 
                    new DriveOption(letter, options.DrivesToIndex.Contains(letter)))
        );

        ShowInTrayOption   = new ShowInTrayOption { ShowInTray = options.ShowInTray };
        IndexedFileCount   = new IndexedFileCount { Count = _fileCountProvider.Count };

        // Commands
        TrayToggledCommand = new RelayCommand(TrayToggled);
        DriveCheckedCommand    = new RelayCommand(DriveChecked);
        IndexFilesCommand = new RelayCommand(() => OnOptionsChanged().ConfigureAwait(false));
        OpenFileOptionsCommand = new RelayCommand(OpenFileOptions);
        OpenDirOptionsCommand  = new RelayCommand(OpenDirOptions);
        ExitCommand            = new RelayCommand(Exit);
    }
    
    private void TrayToggled()
    {
        snackbarService.Show(
            "This option will be applied after the next application restart.",
            "To enable the tray icon edit the configuration file and set ShowInTray to true.",
            ControlAppearance.Info,
            new SymbolIcon(SymbolRegular.Fluent24, 1D), 
            TimeSpan.FromSeconds(4));
        
        _options.ShowInTray = ShowInTrayOption.ShowInTray;
        _configService.Save();
    }

    private void DriveChecked()
    {
        _options.DrivesToIndex.Clear();
        foreach (var d in AllDrives.Where(x => x.Include))
            _options.DrivesToIndex.Add(d.DriveLetter);

        _configService.Save();
        _ = OnOptionsChanged();
    }

    private void OpenFileOptions()
    {
        _configService.Load();
        var clonedOpts = new FileFilterOptions();
        clonedOpts.CopyFrom(_options.FileFilter);
        var dlg = new FileFilterOptionsViewModel(clonedOpts);
        if (_dialogService.ShowDialog(dlg))
        {
            _options.FileFilter.CopyFrom(clonedOpts);
            _configService.Save();
        }

        _ = OnOptionsChanged();
    }

    private void OpenDirOptions()
    {
        _configService.Load();
        var clonedOpts = new DirectoryFilterOptions();
        clonedOpts.CopyFrom(_options.DirectoryFilter);
        var dlg = new DirectoryFilterOptionsViewModel(clonedOpts);
        if (_dialogService.ShowDialog(dlg))
        {
            _options.DirectoryFilter.CopyFrom(clonedOpts);
            _configService.Save();
        }
        _ = OnOptionsChanged();
    }

    private void Exit()
    {
        _configService.Save();
        Application.Current.Shutdown();
    }
    public event PropertyChangedEventHandler? PropertyChanged;
}