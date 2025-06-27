using System.Collections.Immutable;
using System.IO;
using NanoSearch.Algorithms.RadixTrie;

namespace NanoSearch;
public interface ISearchService : IFileCountProvider
{
    IEnumerable<AppSearchResult> Search(string query);
}

public sealed class SearchService : ISearchService
{
    private readonly RadixTree<ImmutableHashSet<string>> _indexedFiles;
    private readonly IAppLauncher _explorerLauncher;
    private readonly IIconLoader _iconLoader;
    public ulong Count => _indexedFiles.Count;
    
    
    public SearchService(RadixTree<ImmutableHashSet<string>> indexedFiles, IAppLauncher explorerLauncher, IIconLoader iconLoader)
    {
        _indexedFiles = indexedFiles;
        _explorerLauncher = explorerLauncher;
        _iconLoader = iconLoader;
    }

    public IEnumerable<AppSearchResult> Search(string query)
    {
        var strategy = SearchStrategyFactory.GetStrategy(query);
        var results = strategy?.Search(_indexedFiles, query);

        if (results == null || results.Count == 0)
        {
            yield return new AppSearchResult(string.Empty, _explorerLauncher, _iconLoader)
            {
                FileName = "No results found"
            };
            yield break;
        }

        foreach (var x in results
                     .Select(path => new { Path = path, FileName = Path.GetFileName(path) })
                     .OrderBy(x => Math.Abs(x.FileName.Length - query.Length))
                     .ThenBy(x => x.FileName, StringComparer.OrdinalIgnoreCase))
        {
            yield return new AppSearchResult(x.Path, _explorerLauncher, _iconLoader);
        }
    }
}