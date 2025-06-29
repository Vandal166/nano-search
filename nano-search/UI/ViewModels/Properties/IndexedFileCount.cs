using System.ComponentModel;

namespace NanoSearch.UI.ViewModels.Properties;

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