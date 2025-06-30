namespace NanoSearch.Navigation.Hotkey;

public interface IHotKeyAction
{
    KeyModifiers Modifiers { get; }
    Keys Key { get; }
    void Execute();
}