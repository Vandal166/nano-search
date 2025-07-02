using NanoSearch.Configuration.Indexing;
using NanoSearch.Configuration.Keybindings;
using NanoSearch.Navigation.Hotkey;

namespace NanoSearch.Configuration.Validators;

public class KeybindingsOptionsValidator : IValidator<KeybindingsOptions>
{
    public IEnumerable<string> Validate(KeybindingsOptions opts)
    {
        var defaults = new KeybindingsOptions();
        
        foreach (var (action, keybind) in opts.Bindings)
        {
            if (action == HotkeyAction.ToggleWindow) //needs to have Modifer and Key set
            {
                if (keybind.Modifiers == KeyModifiers.NoMod)
                {
                    opts.Bindings[action] = new Keybinding
                    {
                        Modifiers = defaults.Bindings[action].Modifiers,
                        Key       = defaults.Bindings[action].Key
                    };
                    yield return $"{KeybindingsOptions.HotkeyToString(action)} action must have a modifier set.";
                }
            }
            else
            {
                if (keybind.Modifiers != KeyModifiers.NoMod)
                {
                    opts.Bindings[action] = new Keybinding
                    {
                        Modifiers = defaults.Bindings[action].Modifiers,
                        Key       = defaults.Bindings[action].Key
                    };
                    yield return $"{KeybindingsOptions.HotkeyToString(action)} action cannot have a modifier set."; //all other actions cannot have an modifier set
                }
                
            }

            if (keybind.Key == Keys.None)
            {
                opts.Bindings[action] = new Keybinding
                {
                    Modifiers = defaults.Bindings[action].Modifiers,
                    Key       = defaults.Bindings[action].Key
                };
                yield return $"{KeybindingsOptions.HotkeyToString(action)} action must have a key set."; //but all must have a key set
            }
        }
    }
}