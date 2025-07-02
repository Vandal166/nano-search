using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NanoSearch.Configuration.Indexing;
using NanoSearch.Configuration.Keybindings;
using NanoSearch.Navigation.Hotkey;

namespace NanoSearch.UI.ViewModels;

public partial class KeybindingItemViewModel : INotifyPropertyChanged
{
    private readonly KeybindingsOptions _options;
    private readonly HotkeyAction       _action;

    //used in displaying the name of the hotkey in xaml window
    public string Name => MyRegex().Replace(_action.ToString(), " $1");

    public KeyModifiers Modifiers
    {
        get => _options.Bindings[_action].Modifiers;
        set
        {
            _options.Bindings[_action].Modifiers = value;
            OnPropertyChanged(nameof(ModifiersPromptText));
        }
    }

    public Keys Key
    {
        get => _options.Bindings[_action].Key;
        set
        {
            _options.Bindings[_action].Key = value;
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
            OnPropertyChanged(nameof(ModifiersPromptText));
        }
    }

    public string KeyPromptText => IsListeningForKey ? "Press a key" : Key.ToString();
    public string ModifiersPromptText => IsListeningForModifiers ? "Press a key" : Modifiers.ToString();

    public ICommand StartListeningOnModifiersCommand { get; }
    public ICommand StartListeningOnKeyCommand { get; }

    public KeybindingItemViewModel(HotkeyAction action, KeybindingsOptions options)
    {
        _action  = action;
        _options = options;
    
        StartListeningOnModifiersCommand = new RelayCommand(() => _isListeningForModifiers = true);
        StartListeningOnKeyCommand = new RelayCommand(() => IsListeningForKey = true);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    public void OnPropertyChanged([CallerMemberName] string? n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    
    [System.Text.RegularExpressions.GeneratedRegex("(\\B[A-Z])")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
}