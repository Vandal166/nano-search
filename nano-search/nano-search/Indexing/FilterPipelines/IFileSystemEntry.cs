using System.IO.Enumeration;

namespace NanoSearch.Configuration;

public interface IFileSystemEntry
{
    bool ShouldSkip(ref FileSystemEntry entry);
}