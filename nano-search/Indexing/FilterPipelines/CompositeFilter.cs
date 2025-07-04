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
        for (int i = 0; i < _filters.Count; i++) // using for loop to avoid boxing since _filters is a list of interfaces
        {
            if (_filters[i].ShouldSkip(ref entry))
                return true;
        }
        return false;
    }
}