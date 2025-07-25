﻿using System.IO.Enumeration;

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
        ReadOnlySpan<char> fileName = entry.FileName;
        int lastDot = fileName.LastIndexOf('.');
        if (lastDot < 0)
            return true; // No extension, skip
        
        ReadOnlySpan<char> ext = fileName.Slice(lastDot);

        foreach (var includedExt in _includedExtensions)
        {
            if (ext.Equals(includedExt, StringComparison.OrdinalIgnoreCase))
                return false;
        }
        return true;
    }
}