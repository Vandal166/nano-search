using System.Collections.Immutable;

namespace NanoSearch;

public class ExactSearchFilter : ISearchFilter
{
    public ImmutableHashSet<string>? Apply(ImmutableHashSet<string>? rawResults, string query)
    {
        return rawResults?.Take(50).ToImmutableHashSet();
    }
}