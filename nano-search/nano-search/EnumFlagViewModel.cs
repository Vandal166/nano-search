using System.ComponentModel;
using System.IO;

namespace NanoSearch;

public class EnumFlagViewModel : INotifyPropertyChanged
{
    private readonly FileAttributes _flag;
    private readonly FilterOptionsViewModel _parent;

    public string Name => _flag.ToString();

    public bool IsChecked
    {
        get
        {
            if (_flag == FileAttributes.None)
                return _parent.AttributesToSkip == FileAttributes.None;

            return _parent.AttributesToSkip.HasFlag(_flag);
        }
        set
        {
            var current = _parent.AttributesToSkip;

            if (_flag == FileAttributes.None)
            {
                if (value)
                    _parent.AttributesToSkip = FileAttributes.None;
                else if (current == FileAttributes.None)
                    _parent.AttributesToSkip = FileAttributes.Normal; // or whatever default

                OnPropertyChanged(nameof(IsChecked));
                return;
            }

            _parent.AttributesToSkip = value
                ? current | _flag
                : current & ~_flag;

            OnPropertyChanged(nameof(IsChecked));
        }
    }

    public EnumFlagViewModel(FileAttributes flag, FilterOptionsViewModel parent)
    {
        _flag = flag;
        _parent = parent;
    }
    
    public void NotifyIsCheckedChanged() => OnPropertyChanged(nameof(IsChecked));
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}