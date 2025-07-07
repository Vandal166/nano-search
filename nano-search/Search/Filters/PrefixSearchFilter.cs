using System.Collections.Immutable;

namespace NanoSearch.Search.Filters;

public class PrefixSearchFilter : ISearchFilter
{
    public ImmutableHashSet<string>? Apply(ImmutableHashSet<string>? rawResults, string query)
    {
        return rawResults?.Take(25)
            .ToImmutableHashSet();
    }
}