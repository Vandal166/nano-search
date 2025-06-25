using System.Collections.Immutable;
using NanoSearch.Algorithms.RadixTrie;

namespace NanoSearch;

public interface ISearchStrategy
{
    ImmutableHashSet<string>? Search(RadixTree<ImmutableHashSet<string>> tree, string query);
    bool CanHandle(string query);
}