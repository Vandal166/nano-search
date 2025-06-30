using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace NanoSearch.Navigation.Hotkey;

public sealed class HotKeyService : IHotKeyService
{
    private const int WM_HOTKEY = 0x0312;
    private int _nextId = 1;
    
    private readonly HwndSource _visual;
    private readonly Dictionary<int, IHotKeyAction> _actions = new Dictionary<int, IHotKeyAction>();
    
    public HotKeyService(Window w, IEnumerable<IHotKeyAction> handlers)
    {
        _visual = HwndSource.FromHwnd(new WindowInteropHelper(w).Handle)
                  ?? throw new InvalidOperationException();

        _visual.AddHook(WndProc);

        foreach (var h in handlers)
        {
            RegisterGlobal(h.Modifiers, h.Key, h);
        }
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