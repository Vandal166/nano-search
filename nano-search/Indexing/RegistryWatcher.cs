using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace NanoSearch;

//TODO use it or remove
class RegistryWatcher : IDisposable
{
    private readonly Thread _watcherThread;
    private readonly string _registryPath;
    private bool _running = true;

    public event Action? OnNewApplication;

    public RegistryWatcher(string registryPath)
    {
        _registryPath = registryPath;
        _watcherThread = new Thread(WatchLoop) { IsBackground = true };
        _watcherThread.Start();
    }

    private void WatchLoop()
    {
        var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
        using (var key = baseKey.OpenSubKey(_registryPath, false))
        {
            var hKey = key.Handle;
            while (_running)
            {
                // Wait for any change in the key (subkey additions/deletions)
                RegNotifyChangeKeyValue(
                    hKey.DangerousGetHandle(),
                    true,
                    RegChangeNotifyFilter.Name,
                    IntPtr.Zero,
                    false);

                // A change occurred
                OnNewApplication?.Invoke();
            }
        }
    }

    public void Dispose()
    {
        _running = false;
        _watcherThread.Join();
    }

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern int RegNotifyChangeKeyValue(
        IntPtr hKey,
        bool bWatchSubtree,
        RegChangeNotifyFilter dwNotifyFilter,
        IntPtr hEvent,
        bool fAsynchronous);

    [Flags]
    private enum RegChangeNotifyFilter : uint
    {
        Name = 1,       // Subkey added or deleted
        Attributes = 2,
        LastSet = 4,    // Value changed
        Security = 8
    }
}