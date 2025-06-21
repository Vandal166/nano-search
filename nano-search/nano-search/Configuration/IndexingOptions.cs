using System.IO;
using System.Text.Json.Serialization;

namespace NanoSearch.Configuration.Indexing;

/// <summary>
/// General configuration filter options for file/directory indexing.
/// </summary>
public sealed class IndexingOptions
{
    [JsonIgnore] 
    public static string? DiskRootPath => Path.GetPathRoot(Environment.CurrentDirectory); // getting root path of curr dir, such as "C:\"
        
    public FileFilterOptions      FileFilter   { get; init; } = new();
    public DirectoryFilterOptions DirectoryFilter    { get; init; } = new();

    public void CopyFrom(IndexingOptions? other)
    {
        if (other == null) 
            return;
        
        FileFilter.CopyFrom(other.FileFilter);
        DirectoryFilter.CopyFrom(other.DirectoryFilter);
    }
}
