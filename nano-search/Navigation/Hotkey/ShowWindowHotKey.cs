using NanoSearch.UI.Windows;

namespace NanoSearch.Navigation.Hotkey;

public class ShowWindowHotKey : IHotKeyAction
{
    private readonly SearchWindow _window;
    public KeyModifiers Modifiers { get; set; } = KeyModifiers.Alt;
    public Keys         Key       { get; set; } = Keys.Space;

    public ShowWindowHotKey(SearchWindow window)
    {
        _window = window;
    }

    public void Execute() => _window.ShowAndFocus();
}