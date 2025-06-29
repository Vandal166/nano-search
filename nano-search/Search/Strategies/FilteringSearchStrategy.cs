using System.Collections.Immutable;
using NanoSearch.Algorithms.RadixTrie;
using NanoSearch.Search.Filters;

namespace NanoSearch.Search.Strategies;

public class FilteringSearchStrategy : ISearchStrategy
{
    private readonly ISearchStrategy _inner;
    private readonly ISearchFilter   _filter;

    public FilteringSearchStrategy(ISearchStrategy inner, ISearchFilter filter)
    {
        _inner  = inner;
        _filter = filter;
    }

    public ImmutableHashSet<string>? Search(RadixTree<ImmutableHashSet<string>> tree, string query)
    {
        var raw = _inner.Search(tree, query);

        // applying the filter, returning the filtered results
        return _filter.Apply(raw, query);
    }

    public bool CanHandle(string query)
    {
        return _inner.CanHandle(query);
    }
}