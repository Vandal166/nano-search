using System.ComponentModel;

namespace NanoSearch.UI.ViewModels.Properties;

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