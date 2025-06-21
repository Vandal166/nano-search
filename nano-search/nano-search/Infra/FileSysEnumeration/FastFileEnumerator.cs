using System.IO;
using System.IO.Enumeration;
using NanoSearch.Configuration;

namespace NanoSearch.Enumerators;

public sealed class FastFileEnumerator : FileSystemEnumerator<string>
{
    private readonly IFileSystemEntry _fileFilter;

    public FastFileEnumerator(string root, IFileSystemEntry fileFilter, FileAttributes fileFilterOptions)
        : base
        (
            root,
            new EnumerationOptions
            {
                IgnoreInaccessible = true, // Skip directories we can't access
                RecurseSubdirectories = false,
                ReturnSpecialDirectories = false,
                AttributesToSkip = fileFilterOptions,
                BufferSize = 65536 // 64KB buffer size
            }
        )
    {
        _fileFilter = fileFilter;
    }

    protected override bool ShouldIncludeEntry(ref FileSystemEntry entry)
    {
        if(entry.IsDirectory)
            return false; // Only process files, not directories
        
        return !_fileFilter.ShouldSkip(ref entry); // Apply the file filter
    }

    protected override string TransformEntry(ref FileSystemEntry entry)
        => entry.FileName.ToString();       // only pull back the filename string
}