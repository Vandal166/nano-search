using System.Text.Json.Serialization;
using NanoSearch.Navigation.Hotkey;

namespace NanoSearch.Configuration.Keybindings;

public sealed partial class KeybindingsOptions
{
    [JsonInclude]
    public Dictionary<HotkeyAction, Keybinding> Bindings { get; set; } 
        = Enum
            .GetValues<HotkeyAction>()
            .ToDictionary(
                a => a,
                a => a switch
                {
                    HotkeyAction.ToggleWindow   => new Keybinding { Modifiers = KeyModifiers.Alt, Key = Keys.Space },
                    HotkeyAction.NavigateDown   => new Keybinding { Modifiers = KeyModifiers.NoMod, Key = Keys.Down },
                    HotkeyAction.NavigateUp     => new Keybinding { Modifiers = KeyModifiers.NoMod, Key = Keys.Up },
                    HotkeyAction.LaunchSelection=> new Keybinding { Modifiers = KeyModifiers.NoMod, Key = Keys.Enter },
                    _                           => new Keybinding()
                });

    public void CopyFrom(KeybindingsOptions other)
    {
        foreach (var kv in other.Bindings)
            Bindings[kv.Key] = new Keybinding 
                { Modifiers = kv.Value.Modifiers, Key = kv.Value.Key };
    }

    public static string HotkeyToString(HotkeyAction action)
    {
        return MyRegex().Replace(action.ToString(), " $1");
    }

    [System.Text.RegularExpressions.GeneratedRegex("(\\B[A-Z])")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
}
