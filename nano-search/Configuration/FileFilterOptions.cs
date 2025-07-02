using System.IO;
using System.Text.Json.Serialization;

namespace NanoSearch.Configuration.Indexing;

public sealed class FileFilterOptions
{
    [JsonInclude]
    public FileAttributes AttributesToSkip { get; set; } = 
        FileAttributes.System | FileAttributes.Temporary | FileAttributes.ReparsePoint;
    
    [JsonInclude]
    public string RegexNamePattern { get; set; } = @"^[\p{L}][\p{L}\p{N}_.-]*$";
    
    [JsonInclude]
    public HashSet<char> ExcludeBeginningWith { get; set; } = new HashSet<char>
    {
        '.',
        '~',
        '#',
        '$',
        '@',
        '-',
        '{'
    };
 
    // File types to specifically include
    [JsonInclude]
    public HashSet<string> IncludedFileExtensions { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".exe",
    };
    
    public void CopyFrom(FileFilterOptions? other)
    {
        if (other == null) 
            return;

        AttributesToSkip = other.AttributesToSkip;
        RegexNamePattern = other.RegexNamePattern;
        ExcludeBeginningWith.Clear();
        foreach (var c in other.ExcludeBeginningWith)
        {
            ExcludeBeginningWith.Add(c);
        }
        
        IncludedFileExtensions.Clear();
        foreach (var ext in other.IncludedFileExtensions)
        {
            IncludedFileExtensions.Add(ext);
        }
    }
}