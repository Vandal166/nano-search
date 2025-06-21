using System.IO;
using System.IO.Enumeration;

namespace NanoSearch.Configuration.Filters;

public class AttributeFilter : IFileSystemEntry
{
    private readonly FileAttributes _attributesToSkip;
    public AttributeFilter(FileAttributes attributesToSkip)
    {
        _attributesToSkip = attributesToSkip;
    }
    public bool ShouldSkip(ref FileSystemEntry entry) => (entry.Attributes & _attributesToSkip) != 0;
}