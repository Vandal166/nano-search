using System.IO;
using System.IO.Enumeration;
using NanoSearch.Configuration;

namespace NanoSearch.Enumerators;

public sealed class FastDirectoryEnumerator : FileSystemEnumerator<string>
{
    private readonly IFileSystemEntry _directoryFilter;

    public FastDirectoryEnumerator(string root, IFileSystemEntry directoryFilter, FileAttributes directoryFilterOptions)
        : base
        (
            root,
            new EnumerationOptions
            {
                IgnoreInaccessible = true, // Skip directories we can't access
                RecurseSubdirectories = false,
                ReturnSpecialDirectories = false,
                AttributesToSkip = directoryFilterOptions,
                BufferSize = 65536 // 64KB buffer size
            }
        )
    {
        _directoryFilter = directoryFilter;
    }

    protected override bool ShouldIncludeEntry(ref FileSystemEntry entry)
    {
        if (!entry.IsDirectory)
            return false; // Only process directories
        
        return !_directoryFilter.ShouldSkip(ref entry); // Apply the directory filter
    }

    protected override string TransformEntry(ref FileSystemEntry entry)
        => entry.FileName.ToString();
}