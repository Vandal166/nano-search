using System.Collections.Immutable;
using System.Text.RegularExpressions;
using NanoSearch.Algorithms.RadixTrie;

namespace NanoSearch.Search.Strategies;

public sealed class RegexSearchStrategy : ISearchStrategy
{
    public ImmutableHashSet<string>? Search(RadixTree<ImmutableHashSet<string>> tree, string query)
    {
        try
        {
            string regexPattern = ConvertToRegex(query);
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

            var results = tree.SearchRegex(regex)
                .SelectMany(pathSet => pathSet)
                .ToImmutableHashSet();
            
            return results;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
    public bool CanHandle(string query)
    {
        return IsRegexPattern(query);
    }
    private static bool IsRegexPattern(string input)
    {
        return input.Contains('*') || 
               input.Contains('?') || 
               input.Contains('[') || 
               input.Contains(']') || 
               input.Contains('^') || 
               input.Contains('$');
    }
    private static string ConvertToRegex(string pattern)
    {
        // Escape regex metacharacters, then replace wildcards
        string regexPattern = "^" + Regex.Escape(pattern)
            .Replace(@"\*", ".*")
            .Replace(@"\?", ".") + "$";
        return regexPattern;
    }
}