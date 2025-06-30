namespace NanoSearch.Navigation.Hotkey;

public interface IHotKeyService : IDisposable
{
    void RegisterGlobal(KeyModifiers mods, Keys key, IHotKeyAction callback);
}