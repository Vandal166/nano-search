using NanoSearch.Configuration.Indexing;

namespace NanoSearch.Configuration.Validators;

public class FileFilterOptionsValidator : IValidator<FileFilterOptions>
{
    public IEnumerable<string> Validate(FileFilterOptions opts)
    {
        if (!opts.RegexNamePattern.IsValidRegex(out var regexError))
        {
            yield return $"Invalid file‑name regex: {regexError}";
            opts.RegexNamePattern = @"^[\p{L}][\p{L}\p{N}_.-]*$"; // default pattern
        }

        foreach (var ch in opts.ExcludeBeginningWith)
        {
            if(char.IsWhiteSpace(ch))
            {
                yield return "ExcludeBeginningWith cannot contain whitespace characters.";
                opts.ExcludeBeginningWith.Remove(ch);
            }
        }

        foreach (var ext in opts.IncludedFileExtensions)
        {
            if (!ext.HasFileExtension())
            {
                yield return $"Included file extension '{ext}' is invalid. It must start with a dot and contain at least one character after the dot.";
                opts.IncludedFileExtensions.Remove(ext);
            }
        }
    }
}