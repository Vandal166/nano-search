using System.IO;

namespace NanoSearch.Configuration;

/// <summary>
/// Represents a filter pipeline for file system entries
/// </summary>
public class FilterPipeline
{
    public IFileSystemEntry      FileFilter      { get; }
    public IFileSystemEntry      DirectoryFilter { get; }
    public FileAttributes        FileAttributesToSkip { get; }
    public FileAttributes        DirectoryAttributesToSkip { get; }

    public FilterPipeline(
        IFileSystemEntry fileFilter,
        IFileSystemEntry dirFilter, FileAttributes fileAttributesToSkip, FileAttributes directoryAttributesToSkip)
    {
        FileFilter      = fileFilter;
        DirectoryFilter = dirFilter;
        FileAttributesToSkip = fileAttributesToSkip;
        DirectoryAttributesToSkip = directoryAttributesToSkip;
    }
}