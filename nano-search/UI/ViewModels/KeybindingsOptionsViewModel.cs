using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using NanoSearch.Configuration.Indexing;
using NanoSearch.Navigation.Hotkey;

namespace NanoSearch.UI.ViewModels;

public class KeybindingsOptionsViewModel : INotifyPropertyChanged
{
    private readonly KeybindingsOptions _options;
    public ObservableCollection<KeybindingItemViewModel> Keybindings { get; }

    public KeybindingsOptionsViewModel(KeybindingsOptions options)
    {
        _options = options;
        
        Keybindings = new ObservableCollection<KeybindingItemViewModel>
        {
            new KeybindingItemViewModel("Toggle Window", _options.ToggleWindowModifiers, _options.ToggleWindowKey),
            // Add other keybindings here
        };
    }
    
    private void ResetToDefault()
    {
        var def = new KeybindingsOptions();
        _options.CopyFrom(def);
    }

   

    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class KeybindingItemViewModel : INotifyPropertyChanged
{
    public string Name { get; }
    private KeyModifiers _modifiers;
    public KeyModifiers Modifiers
    {
        get => _modifiers;
        set
        {
            _modifiers = value;
            OnPropertyChanged(nameof(Modifiers));
            OnPropertyChanged(nameof(ModifiersPromptText));
        }
    }
    private Keys _key;
    public Keys Key
    {
        get => _key;
        set
        {
            _key = value;
            OnPropertyChanged(nameof(Key));
            OnPropertyChanged(nameof(KeyPromptText));
        }
    }

    private bool _isListeningForKey;
    public bool IsListeningForKey
    {
        get => _isListeningForKey;
        set
        {
            _isListeningForKey = value;
            OnPropertyChanged(nameof(IsListeningForKey));
            OnPropertyChanged(nameof(KeyPromptText));
        }
    }

    private bool _isListeningForModifiers;
    public bool IsListeningForModifiers
    {
        get => _isListeningForModifiers;
        set
        {
            _isListeningForModifiers = value;
            OnPropertyChanged(nameof(IsListeningForModifiers));
            OnPropertyChanged(nameof(ModifiersPromptText));
        }
    }

    public string KeyPromptText => IsListeningForKey ? "Press a key" : Key.ToString();
    public string ModifiersPromptText => IsListeningForModifiers ? "Press a key" : Modifiers.ToString();

    public ICommand StartListeningOnModifiersCommand { get; }
    public ICommand StartListeningOnKeyCommand { get; }

    public KeybindingItemViewModel(string name, KeyModifiers modifiers, Keys key)
    {
        Name = name;
        _modifiers = modifiers;
        _key = key;
        
        StartListeningOnModifiersCommand = new RelayCommand(() => _isListeningForModifiers = true);
        StartListeningOnKeyCommand = new RelayCommand(() => IsListeningForKey = true);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


public class ListeningToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isListening = value is bool b && b;
        return isListening ? Brushes.Yellow : Brushes.LightGray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class KeyToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Keys key)
            return key.ToString();
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}