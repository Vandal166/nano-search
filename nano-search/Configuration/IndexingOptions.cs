using System.ComponentModel;
using System.IO;
using System.Text.Json.Serialization;

namespace NanoSearch.Configuration.Indexing;

/// <summary>
/// General configuration filter options for file/directory indexing.
/// </summary>
public sealed class IndexingOptions
{
    [JsonInclude]
    public bool ShowInTray { get; set; } = true;
    
    [JsonInclude]
    public HashSet<string> DrivesToIndex { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        Path.GetPathRoot(Environment.CurrentDirectory)
        /*"C:\\",
        "E:\\",
        "F:\\"*/
    };
    
    
    public FileFilterOptions      FileFilter   { get; init; } = new();
    public DirectoryFilterOptions DirectoryFilter    { get; init; } = new();
    
    public void CopyFrom(IndexingOptions? other)
    {
        if (other == null) 
            return;
        
        ShowInTray = other.ShowInTray;
        DrivesToIndex.Clear();
        foreach (var drive in other.DrivesToIndex)
        {
            DrivesToIndex.Add(drive);
        }
        
        FileFilter.CopyFrom(other.FileFilter);
        DirectoryFilter.CopyFrom(other.DirectoryFilter);
    }
}
