using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using NanoSearch.Configuration;
using NanoSearch.Configuration.Indexing;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;
// ReSharper disable MemberCanBePrivate.Global

namespace NanoSearch;

public partial class SettingsWindow : Window
{
    public ObservableCollection<DriveOption> AllDrives { get; } = new ObservableCollection<DriveOption>();
    public IndexedFileCount IndexedFileCount { get; } = new IndexedFileCount(); // Do not make private, used in XAML
    public ShowInTrayOption ShowInTrayOption { get; } = new ShowInTrayOption();
    
    private readonly IndexingOptions _options;
    private readonly JsonConfigService _configService;
    private readonly IFileCountProvider _fileCountProvider;
    private readonly Action _onOptionsChanged;
    
    public SettingsWindow(IndexingOptions opts, JsonConfigService cfg, IFileCountProvider fileCountProvider, Action onOptionsChanged)
    {
        InitializeComponent();
        _options = opts;
        _configService = cfg;
        _fileCountProvider = fileCountProvider;
        _onOptionsChanged = onOptionsChanged;
        
        // getting every logical drive on the system
        var systemDrives = DriveInfo.GetDrives()
            .Where(d => d.DriveType is DriveType.Fixed or DriveType.Removable)
            .Select(d => d.Name) // e.g. "C:\"
            .ToList();

        // seeing if it's in the saved config
        var included = new HashSet<string>(_options.DrivesToIndex, StringComparer.OrdinalIgnoreCase);
        foreach (var drive in systemDrives)
        {
            AllDrives.Add(new DriveOption(drive, included.Contains(drive)));
        }

        ShowInTrayOption.ShowInTray = _options.ShowInTray;
        IndexedFileCount.Count = _fileCountProvider.Count;
        DataContext = this;

        Deactivated += (s, e) =>
        {
            Hide();
        };
    }

    private void OnDriveChecked(object s, RoutedEventArgs e)
    {
        _options.DrivesToIndex.Clear();
        foreach (var d in AllDrives.Where(x => x.Include))
            _options.DrivesToIndex.Add(d.DriveLetter);

        _configService.Save();
        _onOptionsChanged();
        IndexedFileCount.Count = _fileCountProvider.Count;
    }

    private void OnOpenFileOptions(object s, RoutedEventArgs e)
    {
        _configService.Load();
        var dlg = new FilterOptionsWindow(_options.FileFilter)
        {
            Owner = this
        };
        if (dlg.ShowDialog() == true)
        {
            _configService.Save();
        }
        _onOptionsChanged();
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

    private void IndexFiles_Click(object sender, RoutedEventArgs e)
    {
        _onOptionsChanged();
    }

    private void ShowInTray_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("This option will be applied after the next application restart.\nTo re-enable edit the configuration file", "Information", 
            System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
       _options.ShowInTray = ShowInTrayOption.ShowInTray;
       _configService.Save();
    }
}

public class DriveOption : INotifyPropertyChanged
{
    public string DriveLetter { get; }
    private bool _include;
    public bool Include
    {
        get => _include;
        set
        {
            if (_include != value)
            {
                _include = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Include)));
            }
        }
    }

    public DriveOption(string driveLetter, bool include)
    {
        DriveLetter = driveLetter;
        _include     = include;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public class IndexedFileCount : INotifyPropertyChanged
{
    private ulong _count;
    public ulong Count
    {
        get => _count;
        set
        {
            if (_count != value)
            {
                _count = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public class ShowInTrayOption : INotifyPropertyChanged
{
    private bool _showInTray = true;
    public bool ShowInTray
    {
        get => _showInTray;
        set
        {
            if (_showInTray != value)
            {
                _showInTray = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowInTray)));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
