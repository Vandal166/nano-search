using System.Windows.Media;

namespace NanoSearch.Services;

public interface IIconLoader
{
    Task<ImageSource?> LoadIconAsync(string fullPath);
}