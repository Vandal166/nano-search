using NanoSearch.Configuration.Indexing;

namespace NanoSearch.Configuration.Validators;

public class DirectoryFilterOptionsValidator : IValidator<DirectoryFilterOptions>
{
    public IEnumerable<string> Validate(DirectoryFilterOptions opts)
    {
        if (!opts.RegexNamePattern.IsValidRegex(out var regexError))
        {
            yield return $"Invalid file‑name regex: {regexError}";
            opts.RegexNamePattern = @"^[\p{L}][\p{L}\p{N}_.-]*$"; // default pattern
        }
        
        foreach (var ch in opts.ExcludeBeginningWith)
        {
            if (char.IsWhiteSpace(ch))
            {
                yield return "ExcludeBeginningWith cannot contain whitespace characters.";
                opts.ExcludeBeginningWith.Remove(ch);
            }
        }
        
        foreach (var name in opts.ExcludedDirectoryNames)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                yield return "Excluded directory names cannot be empty or whitespace.";
                opts.ExcludedDirectoryNames.Remove(name);
            }
        }
    }
}