using System.Windows.Input;
using NanoSearch.Navigation.Hotkey;

namespace NanoSearch.Configuration.Keybindings;

public class Keybinding
{
    public KeyModifiers Modifiers { get; set; }
    public Keys         Key       { get; set; }
    
    public Key ToWpfKey()
    {
        return KeyInterop.KeyFromVirtualKey((int)Key);
    }
}