using System.Collections.Immutable;
using NanoSearch.Algorithms.RadixTrie;

namespace NanoSearch.Search.Strategies;

public interface ISearchStrategy
{
    ImmutableHashSet<string>? Search(RadixTree<ImmutableHashSet<string>> tree, string query);
    bool CanHandle(string query);
}