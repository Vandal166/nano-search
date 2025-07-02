using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using NanoSearch.Configuration;
using NanoSearch.Configuration.Indexing;
using NanoSearch.Configuration.Keybindings;

namespace NanoSearch.Navigation.Hotkey;

public sealed class HotKeyService : IHotKeyService
{
    private readonly IConfigService<KeybindingsOptions> _kbConfig;
    private readonly Func<HotkeyAction, IHotKeyAction> _actionFactory;
    private const int WM_HOTKEY = 0x0312;
    private int _nextId = 1;
    
    private readonly HwndSource _visual;
    private readonly Dictionary<int, IHotKeyAction> _actions = new Dictionary<int, IHotKeyAction>();
    
    public HotKeyService(Window w, IConfigService<KeybindingsOptions> kbConfig, Func<HotkeyAction, IHotKeyAction> actionFactory)
    {
        _kbConfig = kbConfig;
        _actionFactory = actionFactory;
        _visual = HwndSource.FromHwnd(new WindowInteropHelper(w).Handle)
                  ?? throw new InvalidOperationException();

        _visual.AddHook(WndProc);
        _kbConfig.OptionsChanged += (_, _) => RebuildHotkeys();
        
        RebuildHotkeys();
    }
    public void RebuildHotkeys()
    {
        UnregisterAll();
        //could later be extended to register more hotkeys
        var handler = _actionFactory(HotkeyAction.ToggleWindow);
        var binding = _kbConfig.Options.Bindings[HotkeyAction.ToggleWindow];
        RegisterGlobal(binding.Modifiers, binding.Key, handler);
    }
    
    public void RegisterGlobal(KeyModifiers mods, Keys key, IHotKeyAction callback)
    {
        var id = _nextId++;
        var handle = _visual.Handle;
        if (!RegisterHotKey(handle, id, (int)mods, (int)key))
            throw new InvalidOperationException($"Failed to register hotkey {mods} + {key}");
        _actions[id] = callback;
        Console.WriteLine($"Registered hotkey: {mods} + {key} with ID {id}");
    }
    
    public void UnregisterAll()
    {
        var handle = _visual.Handle;
        foreach (var id in _actions.Keys)
        {
            if (!UnregisterHotKey(handle, id))
                throw new InvalidOperationException($"Failed to unregister hotkey with ID {id}");
        }
        _actions.Clear();
        Console.WriteLine("Unregistered all hotkeys.");
    }
    
    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_HOTKEY && _actions.TryGetValue(wParam.ToInt32(), out var action))
        {
            action.Execute();
            handled = true;
        }

        return IntPtr.Zero;
    }
    
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
    [DllImport("user32.dll")]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public void Dispose()
    {
        var handle = _visual.Handle;
        foreach (var id in _actions.Keys)
            UnregisterHotKey(handle, id);
        _visual.RemoveHook(WndProc);
        _actions.Clear();
    }
}