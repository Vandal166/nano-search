using System.IO;
using System.Text.Json.Serialization;

namespace NanoSearch.Configuration.Indexing;

public sealed class DirectoryFilterOptions
{
    [JsonInclude]
    public FileAttributes AttributesToSkip { get; set; } = 
        FileAttributes.System | FileAttributes.Hidden | FileAttributes.Temporary | FileAttributes.ReparsePoint;
    
    [JsonInclude]
    public string RegexNamePattern { get; set; } = @"^[\p{L}\p{N} .()\-\+]+$";
    
    [JsonInclude]
    public HashSet<char> ExcludeBeginningWith { get; } = new HashSet<char>
    {
        '.',
        '~',
        '#',
        '$',
        '@',
        '-',
        '{'
    };
    [JsonInclude]
    public HashSet<string> ExcludedDirectoryNames { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "System Volume Information",
        "$RECYCLE.BIN",
        "System",
        "System32",
        "System64",
        "Windows",
        "Windows Defender",
        "WindowsApps",
        "Windows Security",
        "Windows NT",
        "Windows Mail",
        "WindowsPowerShell",
        "Recovery",
        "Temp",
        "WinSxS",
        "LocalLow",
        "node_modules",
        "Debug",
        "obj",
        "Cache",
        "Backup",
        ".git",
        ".vscode",
        ".idea",
        ".npm",
        ".cache",
        ".config"
    };
    
    public void CopyFrom(DirectoryFilterOptions? other)
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
        
        ExcludedDirectoryNames.Clear();
        foreach (var name in other.ExcludedDirectoryNames)
        {
            ExcludedDirectoryNames.Add(name);
        }
    }
}