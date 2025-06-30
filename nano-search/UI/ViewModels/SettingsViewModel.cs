using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using NanoSearch.Configuration;
using NanoSearch.Configuration.Indexing;
using NanoSearch.Navigation.Hotkey;
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
    private readonly IConfigService<IndexingOptions> _indexingConfig;
    private readonly IConfigService<KeybindingsOptions> _keybindingsConfig;
    
    public ICommand TrayToggledCommand        { get; }
    public ICommand DriveCheckedCommand       { get; }
    public ICommand IndexFilesCommand         { get; }
    public ICommand OpenKeybindingsCommand        { get; }
    public ICommand OpenFileOptionsCommand    { get; }
    public ICommand OpenDirOptionsCommand     { get; }
    public ICommand ExitCommand               { get; }
    
    private readonly IDialogService _dialogService;
    /*private readonly IndexingOptions _options;
    private readonly JsonConfigService _configService;*/
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
    public SettingsViewModel(IDialogService dialogService, IConfigService<IndexingOptions> indexingConfig,
        IConfigService<KeybindingsOptions> keybindingConfig, IFileCountProvider  fileCountProvider, Action onOptionsChanged)
    {
        _dialogService          = dialogService;
        //_options            = options;
        _indexingConfig     = indexingConfig;
        _keybindingsConfig  = keybindingConfig;
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
                    new DriveOption(letter, _indexingConfig.Options.DrivesToIndex.Contains(letter)))
        );

        ShowInTrayOption   = new ShowInTrayOption { ShowInTray = _indexingConfig.Options.ShowInTray };
        IndexedFileCount   = new IndexedFileCount { Count = _fileCountProvider.Count };

        // Commands
        TrayToggledCommand = new RelayCommand(TrayToggled);
        DriveCheckedCommand    = new RelayCommand(DriveChecked);
        IndexFilesCommand = new RelayCommand(() => OnOptionsChanged().ConfigureAwait(false));
        OpenKeybindingsCommand = new RelayCommand(OpenKeybindingsOptions);
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
        
        _indexingConfig.Options.ShowInTray = ShowInTrayOption.ShowInTray;
        _indexingConfig.Save();
    }

    private void DriveChecked()
    {
        _indexingConfig.Options.DrivesToIndex.Clear();
        foreach (var d in AllDrives.Where(x => x.Include))
            _indexingConfig.Options.DrivesToIndex.Add(d.DriveLetter);

        _indexingConfig.Save();
        _ = OnOptionsChanged();
    }

    private void OpenKeybindingsOptions()
    {
        _keybindingsConfig.Load();
        var clonedOpts = new KeybindingsOptions();
        clonedOpts.CopyFrom(_keybindingsConfig.Options);
        var dlg = new KeybindingsOptionsViewModel(clonedOpts);
        if (_dialogService.ShowDialog(dlg))
        {
            _keybindingsConfig.Options.CopyFrom(clonedOpts);
            _keybindingsConfig.Save();
        }
        _ = OnOptionsChanged();
    }
    
    private void OpenFileOptions()
    {
        _indexingConfig.Load();
        var clonedOpts = new FileFilterOptions();
        clonedOpts.CopyFrom(_indexingConfig.Options.FileFilter);
        var dlg = new FileFilterOptionsViewModel(clonedOpts);
        if (_dialogService.ShowDialog(dlg))
        {
            _indexingConfig.Options.FileFilter.CopyFrom(clonedOpts);
            _indexingConfig.Save();
        }

        _ = OnOptionsChanged();
    }

    private void OpenDirOptions()
    {
        _indexingConfig.Load();
        var clonedOpts = new DirectoryFilterOptions();
        clonedOpts.CopyFrom(_indexingConfig.Options.DirectoryFilter);
        var dlg = new DirectoryFilterOptionsViewModel(clonedOpts);
        if (_dialogService.ShowDialog(dlg))
        {
            _indexingConfig.Options.DirectoryFilter.CopyFrom(clonedOpts);
            _indexingConfig.Save();
        }
        _ = OnOptionsChanged();
    }

    private void Exit()
    {
        _indexingConfig.Save();
        _keybindingsConfig.Save();
        Application.Current.Shutdown();
    }
    public event PropertyChangedEventHandler? PropertyChanged;
}