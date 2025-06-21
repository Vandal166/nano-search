using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Application = System.Windows.Application;

namespace NanoSearch;

public class AppSearchResult : INotifyPropertyChanged
{
    private static readonly ConcurrentDictionary<string, ImageSource> _iconCache = new();
    private static readonly object _iconLoadLock = new();
    public string FileName { get; set; }
    public string FullPath { get; }
    
    private ImageSource? _icon;
    public ImageSource? Icon
    {
        get => _icon;
        set
        {
            _icon = value; 
            OnPropertyChanged();
        }
    }
    
    public ICommand OpenCommand { get; }

    public AppSearchResult(string fullPath)
    {
        FullPath = fullPath;
        FileName = System.IO.Path.GetFileName(fullPath);
        OpenCommand = new RelayCommand(OpenInExplorer);
        _ = LoadIconAsync();
    }

    private void OpenInExplorer()
    {
        if (File.Exists(FullPath))
        {
            Process.Start("explorer.exe", $"/select,\"{FullPath}\"");
        }
    }

    public async Task LoadIconAsync()
    {
        if (Icon != null) 
            return;
        
       
        if (_iconCache.TryGetValue(FullPath, out var cachedIcon))
        {
            Icon = cachedIcon;
            return;
        }

        try
        {
            // Get icon on background thread
            var iconHandle = await Task.Run(() => 
            {
                try
                {
                    var pathBuilder = new StringBuilder(FullPath, 260);
                    ushort iconIndex = 0;
                    IntPtr hIcon = ExtractAssociatedIconA(IntPtr.Zero, pathBuilder, ref iconIndex);
                    return hIcon;
                }
                catch
                {
                    return IntPtr.Zero;
                }
            });

            if (iconHandle == IntPtr.Zero) return;

            // Create BitmapSource on UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                lock (_iconLoadLock)
                {
                    if (_iconCache.TryGetValue(FullPath, out var existingIcon))
                    {
                        Icon = existingIcon;
                        return;
                    }

                    var bitmapSource = Imaging.CreateBitmapSourceFromHIcon(
                        iconHandle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                    // Clean up icon handle
                    DestroyIcon(iconHandle);

                    bitmapSource.Freeze(); // Make cross-thread safe
                    _iconCache.TryAdd(FullPath, bitmapSource);
                    Icon = bitmapSource;
                }
            });
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            Console.WriteLine($"Error loading icon for {FullPath}: {ex.Message}");
            Icon = null; // Set to null if icon loading fails
        }
    }
    
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool DestroyIcon(IntPtr handle);
    
    [DllImport("shell32.dll", CharSet = CharSet.Ansi)]
    private static extern IntPtr ExtractAssociatedIconA(IntPtr hInst, StringBuilder pszIconPath, ref ushort piIcon);
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? property = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
}

public class ZeroToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int count && count == 0)
            return Visibility.Visible;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}