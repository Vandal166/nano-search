using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using NanoSearch.Configuration;
using NanoSearch.Configuration.Indexing;
using Application = System.Windows.Application;

// ReSharper disable MemberCanBePrivate.Global
namespace NanoSearch;

public interface IDialogService
{
    /// <summary>
    /// Show the dialog for the given view‑model. 
    /// Return true if the user clicked OK, false otherwise.
    /// </summary>
    bool ShowDialog(object viewModel);
}

public class DialogService : IDialogService
{
    private readonly IServiceProvider _provider;
    public DialogService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public bool ShowDialog(object viewModel)
    {
        // Convention: for a FooViewModel, there is a FooWindow in XAML
        var vmType     = viewModel.GetType();
        var viewType   = Type.GetType(vmType.FullName!.Replace("ViewModel", "Window"));
        if (viewType == null)
            throw new InvalidOperationException($"No view found for {vmType.Name}");

        // Use DI to instantiate the Window if needed
        var window = (System.Windows.Window)(_provider.GetService(viewType) 
                              ?? Activator.CreateInstance(viewType)!);
        window.DataContext = viewModel;
        window.Owner       = Application.Current.MainWindow;
        return window.ShowDialog() == true;
    }
}


public class SettingsViewModel : INotifyPropertyChanged
{
    public ObservableCollection<DriveOption> AllDrives { get; }
    public IndexedFileCount IndexedFileCount { get; }
    public ShowInTrayOption ShowInTrayOption { get; }

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

    public SettingsViewModel(IDialogService dialogService, IndexingOptions options, JsonConfigService configService, IFileCountProvider  fileCountProvider, Action onOptionsChanged)
    {
        _dialogService          = dialogService;
        _options            = options;
        _configService             = configService;
        _fileCountProvider  = fileCountProvider;
        _onOptionsChanged   = onOptionsChanged;

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
        IndexFilesCommand      = new RelayCommand(() => _onOptionsChanged());
        OpenFileOptionsCommand = new RelayCommand(OpenFileOptions);
        OpenDirOptionsCommand  = new RelayCommand(OpenDirOptions);
        ExitCommand            = new RelayCommand(Exit);
    }
    
    private void TrayToggled()
    {
        MessageBox.Show("This option will be applied after the next application restart.\nTo re-enable edit the configuration file", "Information", 
            System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        _options.ShowInTray = ShowInTrayOption.ShowInTray;
        _configService.Save();
    }

    private void DriveChecked()
    {
        _options.DrivesToIndex.Clear();
        foreach (var d in AllDrives.Where(x => x.Include))
            _options.DrivesToIndex.Add(d.DriveLetter);

        _configService.Save();
        _onOptionsChanged();
        IndexedFileCount.Count = _fileCountProvider.Count;
    }

    private void OpenFileOptions()
    {
        _configService.Load();
        var clonedOpts = new FileFilterOptions();
        clonedOpts.CopyFrom(_options.FileFilter);
        var dlg = new FilterOptionsViewModel(clonedOpts);
        if (_dialogService.ShowDialog(dlg))
        {
            _options.FileFilter.CopyFrom(clonedOpts);
            _configService.Save();
        }
        _onOptionsChanged();
        IndexedFileCount.Count = _fileCountProvider.Count;
    }

    private void OpenDirOptions()
    {
        
    }

    private void Exit()
    {
        _configService.Save();
        Application.Current.Shutdown();
    }
    public event PropertyChangedEventHandler? PropertyChanged;
}