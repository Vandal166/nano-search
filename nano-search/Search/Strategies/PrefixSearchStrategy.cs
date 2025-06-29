using System.Collections.Immutable;
using NanoSearch.Algorithms.RadixTrie;

namespace NanoSearch.Search.Strategies;

public class PrefixSearchStrategy : ISearchStrategy
{
    public ImmutableHashSet<string>? Search(RadixTree<ImmutableHashSet<string>> tree, string query)
        => tree.SearchPrefix(query);

    public bool CanHandle(string query) => true;
}