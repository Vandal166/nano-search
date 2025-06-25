using System.Windows.Media;

namespace NanoSearch;

public interface IIconLoader
{
    Task<ImageSource?> LoadIconAsync(string fullPath);
}