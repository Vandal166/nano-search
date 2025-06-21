using System.IO.Enumeration;

namespace NanoSearch.Configuration.Filters;

public class FileExtensionFilter : IFileSystemEntry
{
    private readonly HashSet<string> _includedExtensions;
    
    public FileExtensionFilter(HashSet<string> includedExtensions)
    {
        _includedExtensions = includedExtensions;
    }

    public bool ShouldSkip(ref FileSystemEntry entry)
    {
        if (_includedExtensions.Count == 0) 
            return false;
        
        ReadOnlySpan<char> fileName = entry.FileName;
        int lastDot = fileName.LastIndexOf('.');
        if (lastDot < 0)
            return true; // No extension, skip
        
        var ext = fileName.Slice(lastDot).ToString();
        return !_includedExtensions.Contains(ext);
    }
}