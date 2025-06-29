using System.IO.Enumeration;

namespace NanoSearch.Configuration.Filters;

public class PrefixFilter : IFileSystemEntry
{
    private readonly HashSet<char> _excludeBeginningWith;
    public PrefixFilter(HashSet<char> excludeBeginningWith)
    {
        _excludeBeginningWith = excludeBeginningWith;
    }

    public bool ShouldSkip(ref FileSystemEntry entry)
    {
        ReadOnlySpan<char> name = entry.FileName;
        return name.Length > 0 && _excludeBeginningWith.Contains(name[0]);
    }
    
}