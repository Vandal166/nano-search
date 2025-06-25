﻿using System.Collections.Immutable;
using NanoSearch.Algorithms.RadixTrie;

namespace NanoSearch;

public class ExactSearchStrategy : ISearchStrategy
{
    public ImmutableHashSet<string>? Search(RadixTree<ImmutableHashSet<string>> tree, string query)
        => tree.SearchExact(query); //TODO also .Take(MAX_RESULTS) here or inside RadixTree to limit results CONFIGURABLE via conf opts

    public  bool CanHandle(string query)
    {
        return EndsWithExtension(query);
    }
    private static bool EndsWithExtension(string input)
    {
        return input.Contains('.') || 
               input.Length > 3 && char.IsDigit(input[^1]);
    }
}