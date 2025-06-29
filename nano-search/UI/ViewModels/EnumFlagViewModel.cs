using System.ComponentModel;
using System.IO;

namespace NanoSearch.UI.ViewModels;

public class EnumFlagViewModel : INotifyPropertyChanged
{
    private readonly FileAttributes _flag;
    private readonly Func<FileAttributes> _getter;
    private readonly Action<FileAttributes> _setter;
    
    public string Name => _flag.ToString();

    public bool IsChecked
    {
        get
        {
            if (_flag == FileAttributes.None)
                return _getter() == FileAttributes.None;

            return _getter().HasFlag(_flag);
        }
        set
        {
            var current = _getter();

            if (_flag == FileAttributes.None)
            {
                if (value)
                    _setter(FileAttributes.None);
                else if (current == FileAttributes.None)
                    _setter(FileAttributes.Normal); // or whatever default

                OnPropertyChanged(nameof(IsChecked));
                return;
            }

            _setter
            (
                value
                    ? current | _flag
                    : current & ~_flag
            );

            OnPropertyChanged(nameof(IsChecked));
        }
    }

    public EnumFlagViewModel(FileAttributes flag, Func<FileAttributes> getter, Action<FileAttributes> setter)
    {
        _flag = flag;
        _getter = getter;
        _setter = setter;
    }
    
    public void NotifyIsCheckedChanged() => OnPropertyChanged(nameof(IsChecked));
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}