using System.Collections.Immutable;

namespace NanoSearch.Search.Filters;

public class ExactSearchFilter : ISearchFilter
{
    public ImmutableHashSet<string>? Apply(ImmutableHashSet<string>? rawResults, string query)
    {
        return rawResults?.Take(50).ToImmutableHashSet();
    }
}