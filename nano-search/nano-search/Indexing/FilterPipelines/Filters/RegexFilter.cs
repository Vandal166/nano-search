using System.IO.Enumeration;
using System.Text.RegularExpressions;

namespace NanoSearch.Configuration.Filters;

public class RegexFilter : IFileSystemEntry
{
    private readonly Regex _regex;
    public RegexFilter(string pattern, RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase)
    {
        _regex = new Regex(pattern, options);
    }
    
    public bool ShouldSkip(ref FileSystemEntry entry)
    {
        return !_regex.IsMatch(entry.FileName);
    }
}