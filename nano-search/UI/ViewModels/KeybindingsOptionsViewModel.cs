using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using NanoSearch.Configuration.Indexing;
using NanoSearch.Configuration.Keybindings;

namespace NanoSearch.UI.ViewModels;

public class KeybindingsOptionsViewModel : INotifyPropertyChanged
{
    private readonly KeybindingsOptions _options;
    public ObservableCollection<KeybindingItemViewModel> Keybindings { get; }

    public ICommand ResetToDefaultCommand { get; }
    public KeybindingsOptionsViewModel(KeybindingsOptions options)
    {
        _options = options;
        
        Keybindings = new ObservableCollection<KeybindingItemViewModel>(
            Enum.GetValues<HotkeyAction>()
                .Select(a => new KeybindingItemViewModel(a, _options))
        );
        ResetToDefaultCommand = new RelayCommand(ResetToDefault);
    }
    
    private void ResetToDefault()
    {
        var def = new KeybindingsOptions();
        _options.CopyFrom(def);
     
        OnPropertyChanged(nameof(Keybindings));
        foreach (var item in Keybindings)
        {
            item.OnPropertyChanged(nameof(item.Modifiers));
            item.OnPropertyChanged(nameof(item.Key));
            item.OnPropertyChanged(nameof(item.KeyPromptText));
            item.OnPropertyChanged(nameof(item.ModifiersPromptText));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}