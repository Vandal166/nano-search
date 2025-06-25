using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace NanoSearch;

public sealed class HotKeyService : IHotKeyService
{
    private const int WM_HOTKEY = 0x0312;
    private int _nextId = 1;
    
    private readonly HwndSource _visual;
    private readonly Dictionary<int, Action> _actions = new Dictionary<int, Action>();
    
    public HotKeyService(Window window)
    {
        var helper = new WindowInteropHelper(window);
        _visual = HwndSource.FromHwnd(helper.Handle) ?? throw new InvalidOperationException("Could not get HwndSource");
        _visual.AddHook(WndProc);
    }
    public void RegisterGlobal(KeyModifiers mods, Keys key, Action callback)
    {
        var id = _nextId++;
        var handle = _visual.Handle;
        if (!RegisterHotKey(handle, id, (int)mods, (int)key))
        {
            throw new InvalidOperationException("Failed to register hotkey.");
        }
        _actions[id] = callback;
    }
    
    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_HOTKEY && _actions.TryGetValue(wParam.ToInt32(), out var action))
        {
            action();
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