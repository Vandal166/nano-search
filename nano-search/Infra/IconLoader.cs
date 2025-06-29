using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Application = System.Windows.Application;

namespace NanoSearch.Services;

public class IconLoader : IIconLoader
{
    private static readonly ConcurrentDictionary<string, ImageSource> _iconCache = new();

    public async Task<ImageSource?> LoadIconAsync(string fullPath)
    {
        if (_iconCache.TryGetValue(fullPath, out var cachedIcon))
            return cachedIcon;

        try
        {
            var iconHandle = await Task.Run(() =>
            {
                var pathBuilder = new StringBuilder(fullPath, 260);
                ushort iconIndex = 0;
                return ExtractAssociatedIconA(IntPtr.Zero, pathBuilder, ref iconIndex);
            });

            if (iconHandle == IntPtr.Zero)
                return null;

            ImageSource? bitmapSource = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                bitmapSource = Imaging.CreateBitmapSourceFromHIcon(
                    iconHandle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                DestroyIcon(iconHandle);
                bitmapSource?.Freeze();
                if (bitmapSource != null)
                    _iconCache.TryAdd(fullPath, bitmapSource);
            });

            return bitmapSource;
        }
        catch
        {
            return null;
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool DestroyIcon(IntPtr handle);

    [DllImport("shell32.dll", CharSet = CharSet.Ansi)]
    private static extern IntPtr ExtractAssociatedIconA(IntPtr hInst, StringBuilder pszIconPath, ref ushort piIcon);
}