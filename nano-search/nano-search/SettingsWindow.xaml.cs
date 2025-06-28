using System.ComponentModel;
using System.Windows;

namespace NanoSearch;

public partial class SettingsWindow : Window
{
    public SettingsWindow(SettingsViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        
        Deactivated += (s, e) =>
        {
            Hide();
        };
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
