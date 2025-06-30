using NanoSearch.Navigation.Hotkey;

namespace NanoSearch.Configuration.Indexing;

public sealed class KeybindingsOptions
{
    public KeyModifiers ToggleWindowModifiers { get; set; } = KeyModifiers.Alt;
    public Keys ToggleWindowKey { get; set; } = Keys.Space;
    
    /*etc for search result navigation*/
    
    public void CopyFrom(KeybindingsOptions? other)
    {
        if (other == null) 
            return;
        
        ToggleWindowModifiers = other.ToggleWindowModifiers;
        ToggleWindowKey = other.ToggleWindowKey;
        
        /*etc for search result navigation*/
    }
}