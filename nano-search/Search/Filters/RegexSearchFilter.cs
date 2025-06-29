using System.Collections.Immutable;

namespace NanoSearch.Search.Filters;

public class RegexSearchFilter : ISearchFilter
{
    public ImmutableHashSet<string>? Apply(ImmutableHashSet<string>? rawResults, string query)
    {
        // e.g. keep only the first N matches
        return rawResults?.Take(50)
            .ToImmutableHashSet();
    }
}