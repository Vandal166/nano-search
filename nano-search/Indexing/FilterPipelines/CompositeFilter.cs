using System.IO.Enumeration;

namespace NanoSearch.Configuration.Filters;

/// <summary>
/// Used to apply filters to an entry
/// </summary>
public class CompositeFilter: IFileSystemEntry
{
    private readonly IReadOnlyList<IFileSystemEntry> _filters;
    public CompositeFilter(IEnumerable<IFileSystemEntry> filters)
    {
        _filters = filters.ToList().AsReadOnly();
    }

    public bool ShouldSkip(ref FileSystemEntry entry)
    {
        foreach (var filter in _filters)
        {
            if (filter.ShouldSkip(ref entry))
                return true;
        }
        return false;
    }
}