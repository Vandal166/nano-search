using System.ComponentModel;

namespace NanoSearch.UI.ViewModels.Properties;

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