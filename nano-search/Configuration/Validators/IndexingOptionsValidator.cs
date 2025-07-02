using System.IO;
using NanoSearch.Configuration.Indexing;

namespace NanoSearch.Configuration.Validators;

public class IndexingOptionsValidator : IValidator<IndexingOptions>
{
    private readonly IValidator<FileFilterOptions> _fileValidator;
    private readonly IValidator<DirectoryFilterOptions> _dirValidator;

    public IndexingOptionsValidator(IValidator<FileFilterOptions> fileValidator, IValidator<DirectoryFilterOptions> dirValidator)
    {
        _fileValidator = fileValidator;
        _dirValidator  = dirValidator;
    }
    
    public IEnumerable<string> Validate(IndexingOptions opts)
    {
        foreach (var drive in opts.DrivesToIndex.ToList())
        {
            if (!Directory.Exists(drive))
            {
                yield return $"Drive not found: {drive}";
                opts.DrivesToIndex.Remove(drive);
            }
        }
        
        // delegating to sub‑options
        foreach (var err in _fileValidator.Validate(opts.FileFilter))
            yield return err;

        foreach (var err in _dirValidator.Validate(opts.DirectoryFilter))
            yield return err;
        
    }
}