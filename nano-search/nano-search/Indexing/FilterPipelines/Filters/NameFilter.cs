using System.IO.Enumeration;

namespace NanoSearch.Configuration.Filters;

public class NameFilter: IFileSystemEntry
{
    private readonly HashSet<string> _excludedNames;
    private readonly StringComparison _comparison;
    public NameFilter(HashSet<string> excludedNames, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        _excludedNames = excludedNames;
        _comparison = comparison;
    }

    public bool ShouldSkip(ref FileSystemEntry entry) => _excludedNames.Contains(entry.FileName.ToString()); 
}