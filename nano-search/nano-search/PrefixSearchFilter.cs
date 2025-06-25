using System.Collections.Immutable;

namespace NanoSearch;

public class PrefixSearchFilter : ISearchFilter
{
    public ImmutableHashSet<string>? Apply(ImmutableHashSet<string>? rawResults, string query)
    {
        return rawResults?.Take(50) // TODO configurable via options
            .ToImmutableHashSet();
    }
}