using NanoSearch.Configuration.Filters;
using NanoSearch.Configuration.Indexing;

namespace NanoSearch.Configuration;

/// <summary>
/// Builds a filter pipeline based on the provided indexing options
/// </summary>
public sealed class FilterPipelineBuilder
{
    public static FilterPipeline Build(IndexingOptions opts)
    {
        if (opts is null)
            throw new ArgumentNullException(nameof(opts));
        
        // 1) File filters
        var fileOpt = opts.FileFilter;
        var fileFilters = new List<IFileSystemEntry>
        {
            new PrefixFilter(fileOpt.ExcludeBeginningWith),
            new AttributeFilter(fileOpt.AttributesToSkip),
            new FileExtensionFilter(fileOpt.IncludedFileExtensions),
            new RegexFilter(fileOpt.RegexNamePattern),
        };
        var filePipeline = new CompositeFilter(fileFilters);

        // 2) Directory filters
        var dirOpt = opts.DirectoryFilter;
        var dirFilters = new List<IFileSystemEntry>
        {
            new PrefixFilter(dirOpt.ExcludeBeginningWith),
            new AttributeFilter(dirOpt.AttributesToSkip),
            new NameFilter(dirOpt.ExcludedDirectoryNames),
            new RegexFilter(dirOpt.RegexNamePattern),
        };
        var dirPipeline = new CompositeFilter(dirFilters);
        var fileAttributesToSkip = fileOpt.AttributesToSkip;
        var directoryAttributesToSkip = dirOpt.AttributesToSkip;
        
        return new FilterPipeline(filePipeline, dirPipeline, 
            fileAttributesToSkip, directoryAttributesToSkip);
    }
}