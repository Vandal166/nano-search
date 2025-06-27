using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NanoSearch.Configuration.Indexing;

public sealed class DirectoryFilterOptions : INotifyPropertyChanged
{
    private FileAttributes _attributesToSkip { get; set; } = 
        FileAttributes.System | FileAttributes.Hidden | FileAttributes.Temporary | FileAttributes.ReparsePoint;
    
    public FileAttributes AttributesToSkip
    {
        get => _attributesToSkip;
        set { 
            if (_attributesToSkip != value) 
            {
                _attributesToSkip = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AttributesToSkip)));
            }
        }
    }
        
    
    private string _regexNamePattern { get; set; } = @"^[\p{L}\p{N} .()\-\+]+$";
    public string RegexNamePattern
    {
        get => _regexNamePattern;
        set { 
            if (_regexNamePattern != value) 
            {
                _regexNamePattern = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RegexNamePattern)));
            }
        }
    }
    
    private HashSet<char> _excludeBeginningWith { get; set; } = new HashSet<char>
    {
        '.',
        '~',
        '#',
        '$',
        '@',
        '-',
        '{'
    };
    public HashSet<char> ExcludeBeginningWith
    {
        get => _excludeBeginningWith;
        set
        {
            if (_excludeBeginningWith.SetEquals(value)) 
                return;
            _excludeBeginningWith = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExcludeBeginningWith)));
        }
    }
    
    
    
    private HashSet<string> _excludedDirectoryNames { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
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
    public HashSet<string> ExcludedDirectoryNames
    {
        get => _excludedDirectoryNames;
        set
        {
            if (_excludedDirectoryNames.SetEquals(value)) 
                return;
            _excludedDirectoryNames.Clear();
            foreach (var name in value)
            {
                _excludedDirectoryNames.Add(name);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExcludedDirectoryNames)));
        }
    }
    
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

    public event PropertyChangedEventHandler? PropertyChanged;
}